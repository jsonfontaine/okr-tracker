using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por listar todos os projetos cadastrados.
    /// </summary>
    public class ListarProjetosService : IListarProjetosService
    {
        private readonly IProjetoRepository _projetoRepository;
        private readonly ILogger<ListarProjetosService> _logger;

        public ListarProjetosService(IProjetoRepository projetoRepository, ILogger<ListarProjetosService> logger)
        {
            _projetoRepository = projetoRepository;
            _logger = logger;
        }

        public ResultadoOperacao<IEnumerable<ProjetoResponse>> Executar()
        {
            _logger.LogInformation("Listando todos os projetos.");

            var projetos = _projetoRepository.ObterTodos();
            var response = projetos.Select(p => new ProjetoResponse
            {
                Id = p.Id,
                Nome = p.Nome,
                Descricao = p.Descricao,
                DataCriacao = p.DataCriacao,
                UltimaAtualizacao = p.UltimaAtualizacao
            });

            return ResultadoOperacao<IEnumerable<ProjetoResponse>>.Sucesso(response);
        }
    }
}
