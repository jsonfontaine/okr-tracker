using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Enums;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por excluir um Key Result.
    /// Impede exclusão se for o último KR do objetivo correspondente.
    /// Após exclusão, recalcula o progresso do objetivo.
    /// </summary>
    public class ExcluirKRService : IExcluirKRService
    {
        private readonly IKeyResultRepository _krRepository;
        private readonly IObjetivoRepository _objetivoRepository;
        private readonly ILogger<ExcluirKRService> _logger;

        public ExcluirKRService(
            IKeyResultRepository krRepository,
            IObjetivoRepository objetivoRepository,
            ILogger<ExcluirKRService> logger)
        {
            _krRepository = krRepository;
            _objetivoRepository = objetivoRepository;
            _logger = logger;
        }

        public ResultadoOperacao Executar(string id)
        {
            _logger.LogInformation("Tentando excluir KR {Id}.", id);

            var kr = _krRepository.ObterPorId(id);
            if (kr == null)
                return ResultadoOperacao.Erro("KR não encontrado.");

            var totalKrs = _krRepository.ContarPorObjetivoId(kr.ObjetivoId);
            if (totalKrs <= 1)
                return ResultadoOperacao.Erro("Não é possível excluir o último KR de um objetivo.");

            var objetivoId = kr.ObjetivoId;
            _krRepository.Excluir(id);

            // Recalcula progresso do objetivo após exclusão
            RecalcularProgressoObjetivo(objetivoId);

            _logger.LogInformation("KR {Id} excluído com sucesso.", id);
            return ResultadoOperacao.Sucesso();
        }

        /// <summary>
        /// Recalcula o progresso e status do objetivo com base na média dos KRs restantes.
        /// </summary>
        private void RecalcularProgressoObjetivo(string objetivoId)
        {
            var objetivo = _objetivoRepository.ObterPorId(objetivoId);
            if (objetivo == null) return;

            var krs = _krRepository.ObterPorObjetivoId(objetivoId).ToList();
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

        internal static Status CalcularStatus(double progresso)
        {
            if (progresso <= 0) return Status.NaoIniciado;
            if (progresso >= 100) return Status.Concluido;
            if (progresso >= 50) return Status.EmAndamentoAvancado;
            return Status.EmAndamento;
        }
    }
}
