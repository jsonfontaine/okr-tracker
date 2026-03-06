using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por excluir um Key Result.
    /// Impede exclusão se for o último KR do objetivo correspondente.
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

            _krRepository.Excluir(id);

            _logger.LogInformation("KR {Id} excluído com sucesso.", id);
            return ResultadoOperacao.Sucesso();
        }
    }
}
