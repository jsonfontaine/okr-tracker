using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por atualizar nome/descrição de um projeto existente.
    /// Valida existência, nome obrigatório e unicidade.
    /// </summary>
    public class AtualizarProjetoService : IAtualizarProjetoService
    {
        private readonly IProjetoRepository _projetoRepository;
        private readonly ILogger<AtualizarProjetoService> _logger;

        public AtualizarProjetoService(IProjetoRepository projetoRepository, ILogger<AtualizarProjetoService> logger)
        {
            _projetoRepository = projetoRepository;
            _logger = logger;
        }

        public ResultadoOperacao<ProjetoResponse> Executar(string id, AtualizarProjetoRequest request)
        {
            _logger.LogInformation("Atualizando projeto {Id} para nome: {Nome}", id, request.Nome);

            if (string.IsNullOrWhiteSpace(request.Nome))
                return ResultadoOperacao<ProjetoResponse>.Erro("O nome do projeto é obrigatório.");

            var projeto = _projetoRepository.ObterPorId(id);
            if (projeto == null)
                return ResultadoOperacao<ProjetoResponse>.Erro("Projeto não encontrado.");

            var existente = _projetoRepository.ObterPorNome(request.Nome);
            if (existente != null && existente.Id != id)
                return ResultadoOperacao<ProjetoResponse>.Erro("Já existe um projeto com este nome.");

            projeto.Nome = request.Nome;
            projeto.Descricao = request.Descricao;
            projeto.UltimaAtualizacao = DateTime.UtcNow;

            _projetoRepository.Atualizar(projeto);
            _logger.LogInformation("Projeto {Id} atualizado com sucesso.", id);

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
