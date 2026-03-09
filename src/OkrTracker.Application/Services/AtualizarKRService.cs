using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Enums;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por atualizar campos de um Key Result existente.
    /// Não permite alterar o objetivoId.
    /// Gera comentários automáticos quando Farol ou Status são alterados.
    /// </summary>
    public class AtualizarKRService : IAtualizarKRService
    {
        private readonly IKeyResultRepository _krRepository;
        private readonly IComentarioRepository _comentarioRepository;
        private readonly ILogger<AtualizarKRService> _logger;

        public AtualizarKRService(
            IKeyResultRepository krRepository,
            IComentarioRepository comentarioRepository,
            ILogger<AtualizarKRService> logger)
        {
            _krRepository = krRepository;
            _comentarioRepository = comentarioRepository;
            _logger = logger;
        }

        public ResultadoOperacao<KeyResultResponse> Executar(string id, AtualizarKeyResultRequest request)
        {
            _logger.LogInformation("Atualizando KR {Id}.", id);

            if (string.IsNullOrWhiteSpace(request.Titulo))
                return ResultadoOperacao<KeyResultResponse>.Erro("Título do KR é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.Descricao))
                return ResultadoOperacao<KeyResultResponse>.Erro("Descrição do KR é obrigatória.");

            if (!Enum.TryParse<TipoKR>(request.Tipo, true, out var tipo))
                return ResultadoOperacao<KeyResultResponse>.Erro("Selecione um tipo válido para o KR.");

            var kr = _krRepository.ObterPorId(id);
            if (kr == null)
                return ResultadoOperacao<KeyResultResponse>.Erro("KR não encontrado.");

            if (!Enum.TryParse<Farol>(request.Farol, true, out var farol))
                farol = Farol.Verde;

            if (!Enum.TryParse<Status>(request.Status, true, out var status))
                status = Status.NaoIniciado;

            var alteracoes = new List<string>();

            if (kr.Farol != farol)
                alteracoes.Add($"Farol alterado de {kr.Farol} para {farol}");

            if (kr.Status != status)
                alteracoes.Add($"Status alterado de {kr.Status} para {status}");

            kr.Titulo = request.Titulo;
            kr.Descricao = request.Descricao;
            kr.Tipo = tipo;
            kr.Farol = farol;
            kr.Status = status;
            kr.Intruder = request.Intruder;
            kr.DescobertaTardia = request.DescobertaTardia;
            kr.UltimaAtualizacao = DateTime.UtcNow;

            _krRepository.Atualizar(kr);

            if (alteracoes.Count > 0)
            {
                _comentarioRepository.Inserir(new Comentario
                {
                    Id = Guid.NewGuid().ToString(),
                    KrId = kr.Id,
                    Texto = $"[Alteração automática] {string.Join("; ", alteracoes)}.",
                    DataCriacao = DateTime.UtcNow
                });
            }

            _logger.LogInformation("KR {Id} atualizado com sucesso.", id);

            return ResultadoOperacao<KeyResultResponse>.Sucesso(new KeyResultResponse
            {
                Id = kr.Id,
                ObjetivoId = kr.ObjetivoId,
                Titulo = kr.Titulo,
                Descricao = kr.Descricao,
                Tipo = kr.Tipo.ToString(),
                Progresso = kr.Progresso,
                Status = kr.Status.ToString(),
                Farol = kr.Farol.ToString(),
                Intruder = kr.Intruder,
                DescobertaTardia = kr.DescobertaTardia,
                DataCriacao = kr.DataCriacao,
                UltimaAtualizacao = kr.UltimaAtualizacao
            });
        }
    }
}
