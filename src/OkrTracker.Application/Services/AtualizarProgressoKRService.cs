using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Enums;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por atualizar o progresso de um Key Result.
    /// Valida regras especiais: tipo Requisito só aceita 0 ou 100.
    /// </summary>
    public class AtualizarProgressoKRService : IAtualizarProgressoKRService
    {
        private readonly IKeyResultRepository _krRepository;
        private readonly IObjetivoRepository _objetivoRepository;
        private readonly ILogger<AtualizarProgressoKRService> _logger;

        public AtualizarProgressoKRService(
            IKeyResultRepository krRepository,
            IObjetivoRepository objetivoRepository,
            ILogger<AtualizarProgressoKRService> logger)
        {
            _krRepository = krRepository;
            _objetivoRepository = objetivoRepository;
            _logger = logger;
        }

        public ResultadoOperacao<KeyResultResponse> Executar(string id, AtualizarProgressoRequest request)
        {
            _logger.LogInformation("Atualizando progresso do KR {Id} para {Progresso}%.", id, request.Progresso);

            var kr = _krRepository.ObterPorId(id);
            if (kr == null)
                return ResultadoOperacao<KeyResultResponse>.Erro("KR não encontrado.");

            if (request.Progresso < 0 || request.Progresso > 100)
                return ResultadoOperacao<KeyResultResponse>.Erro("Valor de progresso inválido.");

            if (kr.Tipo == TipoKR.Requisito && request.Progresso != 0 && request.Progresso != 100)
                return ResultadoOperacao<KeyResultResponse>.Erro("Para KR do tipo Requisito, o progresso só pode ser 0 ou 100.");

            kr.Progresso = request.Progresso;
            kr.UltimaAtualizacao = DateTime.UtcNow;

            _krRepository.Atualizar(kr);

            _logger.LogInformation("Progresso do KR {Id} atualizado com sucesso.", id);

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
