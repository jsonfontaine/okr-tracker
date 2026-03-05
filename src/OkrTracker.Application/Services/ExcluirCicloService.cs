using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por excluir um ciclo.
    /// Só permite exclusão se não houver objetivos associados ao ciclo.
    /// </summary>
    public class ExcluirCicloService : IExcluirCicloService
    {
        private readonly ICicloRepository _cicloRepository;
        private readonly IObjetivoRepository _objetivoRepository;
        private readonly ILogger<ExcluirCicloService> _logger;

        public ExcluirCicloService(
            ICicloRepository cicloRepository,
            IObjetivoRepository objetivoRepository,
            ILogger<ExcluirCicloService> logger)
        {
            _cicloRepository = cicloRepository;
            _objetivoRepository = objetivoRepository;
            _logger = logger;
        }

        public ResultadoOperacao Executar(string id)
        {
            _logger.LogInformation("Tentando excluir ciclo {Id}.", id);

            var ciclo = _cicloRepository.ObterPorId(id);
            if (ciclo == null)
                return ResultadoOperacao.Erro("Ciclo não encontrado.");

            if (_objetivoRepository.ExistemObjetivosParaCiclo(id))
                return ResultadoOperacao.Erro("Não é possível excluir o ciclo pois existem objetivos associados.");

            _cicloRepository.Excluir(id);
            _logger.LogInformation("Ciclo {Id} excluído com sucesso.", id);

            return ResultadoOperacao.Sucesso();
        }
    }
}
