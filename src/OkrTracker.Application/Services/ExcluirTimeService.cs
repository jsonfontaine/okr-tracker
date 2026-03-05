using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por excluir um time.
    /// Só permite exclusão se não houver objetivos associados ao time.
    /// </summary>
    public class ExcluirTimeService : IExcluirTimeService
    {
        private readonly ITimeRepository _timeRepository;
        private readonly IObjetivoRepository _objetivoRepository;
        private readonly ILogger<ExcluirTimeService> _logger;

        public ExcluirTimeService(
            ITimeRepository timeRepository,
            IObjetivoRepository objetivoRepository,
            ILogger<ExcluirTimeService> logger)
        {
            _timeRepository = timeRepository;
            _objetivoRepository = objetivoRepository;
            _logger = logger;
        }

        public ResultadoOperacao Executar(string id)
        {
            _logger.LogInformation("Tentando excluir time {Id}.", id);

            var time = _timeRepository.ObterPorId(id);
            if (time == null)
                return ResultadoOperacao.Erro("Time não encontrado.");

            if (_objetivoRepository.ExistemObjetivosParaTime(id))
                return ResultadoOperacao.Erro("Não é possível excluir o time pois existem objetivos associados.");

            _timeRepository.Excluir(id);
            _logger.LogInformation("Time {Id} excluído com sucesso.", id);

            return ResultadoOperacao.Sucesso();
        }
    }
}
