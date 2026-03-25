using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por listar todos os ciclos cadastrados.
    /// </summary>
    public class ListarCiclosService : IListarCiclosService
    {
        private readonly ICicloRepository _cicloRepository;
        private readonly ILogger<ListarCiclosService> _logger;

        public ListarCiclosService(ICicloRepository cicloRepository, ILogger<ListarCiclosService> logger)
        {
            _cicloRepository = cicloRepository;
            _logger = logger;
        }

        public ResultadoOperacao<IEnumerable<CicloResponse>> Executar()
        {
            _logger.LogInformation("Listando todos os ciclos.");

            var ciclos = _cicloRepository.ObterTodos();
            
            // Ordenar por DataInicio crescente (ciclos mais antigos primeiro),
            // fallback para DataCriacao para ciclos sem datas
            var ciclosOrdenados = ciclos
                .OrderBy(c => c.DataInicio ?? c.DataCriacao)
                .ToList();

            var response = ciclosOrdenados.Select(c => new CicloResponse
            {
                Id = c.Id,
                Nome = c.Nome,
                DataInicio = c.DataInicio,
                DataFim = c.DataFim,
                DataCriacao = c.DataCriacao,
                UltimaAtualizacao = c.UltimaAtualizacao
            });

            return ResultadoOperacao<IEnumerable<CicloResponse>>.Sucesso(response);
        }
    }
}
