using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
        /// Serviço responsável por criar um novo projeto.
    /// Valida nome obrigatório e unicidade.
    /// </summary>
    public class CriarProjetoService : ICriarProjetoService
    {
        private readonly IProjetoRepository _projetoRepository;
        private readonly ILogger<CriarProjetoService> _logger;

        public CriarProjetoService(IProjetoRepository projetoRepository, ILogger<CriarProjetoService> logger)
        {
            _projetoRepository = projetoRepository;
            _logger = logger;
        }

        public ResultadoOperacao<ProjetoResponse> Executar(CriarProjetoRequest request)
        {
            _logger.LogInformation("Criando projeto com nome: {Nome}", request.Nome);

            if (string.IsNullOrWhiteSpace(request.Nome))
                return ResultadoOperacao<ProjetoResponse>.Erro("O nome do projeto é obrigatório.");

            var existente = _projetoRepository.ObterPorNome(request.Nome);
            if (existente != null)
                return ResultadoOperacao<ProjetoResponse>.Erro("Já existe um projeto com este nome.");

            var agora = DateTime.UtcNow;
            var projeto = new Projeto
            {
                Id = Guid.NewGuid().ToString(),
                Nome = request.Nome,
                Descricao = request.Descricao,
                DataCriacao = agora,
                UltimaAtualizacao = agora
            };

            _projetoRepository.Inserir(projeto);
            _logger.LogInformation("Projeto criado com sucesso. Id: {Id}", projeto.Id);

            return ResultadoOperacao<ProjetoResponse>.Sucesso(new ProjetoResponse
            {
                Id = projeto.Id,
                Nome = projeto.Nome,
                Descricao = projeto.Descricao,
                DataCriacao = projeto.DataCriacao,
                UltimaAtualizacao = projeto.UltimaAtualizacao
            });
        }
    }
}
