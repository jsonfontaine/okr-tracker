using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por listar todos os times cadastrados.
    /// </summary>
    public class ListarTimesService : IListarTimesService
    {
        private readonly ITimeRepository _timeRepository;
        private readonly ILogger<ListarTimesService> _logger;

        public ListarTimesService(ITimeRepository timeRepository, ILogger<ListarTimesService> logger)
        {
            _timeRepository = timeRepository;
            _logger = logger;
        }

        public ResultadoOperacao<IEnumerable<TimeResponse>> Executar()
        {
            _logger.LogInformation("Listando todos os times.");

            var times = _timeRepository.ObterTodos();
            var response = times.Select(t => new TimeResponse
            {
                Id = t.Id,
                Nome = t.Nome,
                Descricao = t.Descricao,
                DataCriacao = t.DataCriacao,
                UltimaAtualizacao = t.UltimaAtualizacao
            });

            return ResultadoOperacao<IEnumerable<TimeResponse>>.Sucesso(response);
        }
    }
}
