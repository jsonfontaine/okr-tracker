using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Enums;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por atualizar campos de um Key Result existente.
    /// Não permite alterar o objetivoId.
    /// </summary>
    public class AtualizarKRService : IAtualizarKRService
    {
        private readonly IKeyResultRepository _krRepository;
        private readonly ILogger<AtualizarKRService> _logger;

        public AtualizarKRService(IKeyResultRepository krRepository, ILogger<AtualizarKRService> logger)
        {
            _krRepository = krRepository;
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

            kr.Titulo = request.Titulo;
            kr.Descricao = request.Descricao;
            kr.Tipo = tipo;
            kr.Farol = farol;
            kr.Intruder = request.Intruder;
            kr.DescobertaTardia = request.DescobertaTardia;
            kr.UltimaAtualizacao = DateTime.UtcNow;

            _krRepository.Atualizar(kr);
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
