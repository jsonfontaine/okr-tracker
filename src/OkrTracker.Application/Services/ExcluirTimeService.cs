using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por excluir um projeto.
    /// Só permite exclusão se não houver objetivos associados ao projeto.
    /// </summary>
    public class ExcluirProjetoService : IExcluirProjetoService
    {
        private readonly IProjetoRepository _projetoRepository;
        private readonly IObjetivoRepository _objetivoRepository;
        private readonly ILogger<ExcluirProjetoService> _logger;

        public ExcluirProjetoService(
            IProjetoRepository projetoRepository,
            IObjetivoRepository objetivoRepository,
            ILogger<ExcluirProjetoService> logger)
        {
            _projetoRepository = projetoRepository;
            _objetivoRepository = objetivoRepository;
            _logger = logger;
        }

        public ResultadoOperacao Executar(string id)
        {
            _logger.LogInformation("Tentando excluir projeto {Id}.", id);

            var projeto = _projetoRepository.ObterPorId(id);
            if (projeto == null)
                return ResultadoOperacao.Erro("Projeto não encontrado.");

            if (_objetivoRepository.ExistemObjetivosParaProjeto(id))
                return ResultadoOperacao.Erro("Não é possível excluir o projeto pois existem objetivos associados.");

            _projetoRepository.Excluir(id);
            _logger.LogInformation("Projeto {Id} excluído com sucesso.", id);

            return ResultadoOperacao.Sucesso();
        }
    }
}
