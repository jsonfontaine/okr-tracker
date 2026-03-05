using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Enums;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por criar um Key Result vinculado a um objetivo.
    /// Valida campos obrigatórios, tipo e existência do objetivo.
    /// </summary>
    public class CriarKRService : ICriarKRService
    {
        private readonly IKeyResultRepository _krRepository;
        private readonly IObjetivoRepository _objetivoRepository;
        private readonly ILogger<CriarKRService> _logger;

        public CriarKRService(
            IKeyResultRepository krRepository,
            IObjetivoRepository objetivoRepository,
            ILogger<CriarKRService> logger)
        {
            _krRepository = krRepository;
            _objetivoRepository = objetivoRepository;
            _logger = logger;
        }

        public ResultadoOperacao<KeyResultResponse> Executar(CriarKeyResultRequest request)
        {
            _logger.LogInformation("Criando KR: {Titulo} para objetivo {ObjetivoId}", request.Titulo, request.ObjetivoId);

            if (string.IsNullOrWhiteSpace(request.Titulo))
                return ResultadoOperacao<KeyResultResponse>.Erro("Título do KR é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.Descricao))
                return ResultadoOperacao<KeyResultResponse>.Erro("Descrição do KR é obrigatória.");

            if (string.IsNullOrWhiteSpace(request.ObjetivoId))
                return ResultadoOperacao<KeyResultResponse>.Erro("O objetivo é obrigatório.");

            if (!Enum.TryParse<TipoKR>(request.Tipo, true, out var tipo))
                return ResultadoOperacao<KeyResultResponse>.Erro("Selecione um tipo válido para o KR.");

            var objetivo = _objetivoRepository.ObterPorId(request.ObjetivoId);
            if (objetivo == null)
                return ResultadoOperacao<KeyResultResponse>.Erro("Objetivo não encontrado.");

            if (request.Progresso < 0 || request.Progresso > 100)
                return ResultadoOperacao<KeyResultResponse>.Erro("Valor de progresso inválido.");

            if (tipo == TipoKR.Requisito && request.Progresso != 0 && request.Progresso != 100)
                return ResultadoOperacao<KeyResultResponse>.Erro("Para KR do tipo Requisito, o progresso só pode ser 0 ou 100.");

            if (!Enum.TryParse<Farol>(request.Farol, true, out var farol))
                farol = Farol.Verde;

            var agora = DateTime.UtcNow;
            var kr = new KeyResult
            {
                Id = Guid.NewGuid().ToString(),
                ObjetivoId = request.ObjetivoId,
                Titulo = request.Titulo,
                Descricao = request.Descricao,
                Tipo = tipo,
                Progresso = request.Progresso,
                Status = CalcularStatus(request.Progresso),
                Farol = farol,
                Intruder = request.Intruder,
                DescobertaTardia = request.DescobertaTardia,
                DataCriacao = agora,
                UltimaAtualizacao = agora
            };

            _krRepository.Inserir(kr);

            // Recalcula progresso do objetivo
            RecalcularProgressoObjetivo(objetivo);

            _logger.LogInformation("KR criado com sucesso. Id: {Id}", kr.Id);
            return ResultadoOperacao<KeyResultResponse>.Sucesso(MapearResponse(kr));
        }

        /// <summary>
        /// Recalcula o progresso e status do objetivo com base na média dos KRs.
        /// </summary>
        private void RecalcularProgressoObjetivo(Objetivo objetivo)
        {
            var krs = _krRepository.ObterPorObjetivoId(objetivo.Id).ToList();
            if (krs.Count == 0)
            {
                objetivo.Progresso = 0;
                objetivo.Status = Status.NaoIniciado;
            }
            else
            {
                objetivo.Progresso = krs.Average(k => k.Progresso);
                objetivo.Status = CalcularStatus(objetivo.Progresso);
            }

            objetivo.UltimaAtualizacao = DateTime.UtcNow;
            _objetivoRepository.Atualizar(objetivo);
        }

        /// <summary>
        /// Calcula o status com base no valor de progresso.
        /// </summary>
        internal static Status CalcularStatus(double progresso)
        {
            if (progresso <= 0) return Status.NaoIniciado;
            if (progresso >= 100) return Status.Concluido;
            if (progresso >= 50) return Status.EmAndamentoAvancado;
            return Status.EmAndamento;
        }

        private static KeyResultResponse MapearResponse(KeyResult kr)
        {
            return new KeyResultResponse
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
            };
        }
    }
}
