using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por criar um novo ciclo.
    /// Valida nome obrigatório e unicidade.
    /// </summary>
    public class CriarCicloService : ICriarCicloService
    {
        private readonly ICicloRepository _cicloRepository;
        private readonly ILogger<CriarCicloService> _logger;

        public CriarCicloService(ICicloRepository cicloRepository, ILogger<CriarCicloService> logger)
        {
            _cicloRepository = cicloRepository;
            _logger = logger;
        }

        public ResultadoOperacao<CicloResponse> Executar(CriarCicloRequest request)
        {
            _logger.LogInformation("Criando ciclo com nome: {Nome}", request.Nome);

            if (string.IsNullOrWhiteSpace(request.Nome))
                return ResultadoOperacao<CicloResponse>.Erro("O nome do ciclo é obrigatório.");

            // Validar consistência de datas se informadas
            if (request.DataInicio.HasValue && request.DataFim.HasValue)
            {
                if (request.DataInicio.Value > request.DataFim.Value)
                    return ResultadoOperacao<CicloResponse>.Erro("A data de início não pode ser posterior à data de término.");
            }

            var existente = _cicloRepository.ObterPorNome(request.Nome);
            if (existente != null)
                return ResultadoOperacao<CicloResponse>.Erro("Já existe um ciclo com este nome.");

            var agora = DateTime.UtcNow;
            var ciclo = new Ciclo
            {
                Id = Guid.NewGuid().ToString(),
                Nome = request.Nome,
                DataInicio = request.DataInicio,
                DataFim = request.DataFim,
                DataCriacao = agora,
                UltimaAtualizacao = agora
            };

            _cicloRepository.Inserir(ciclo);
            _logger.LogInformation("Ciclo criado com sucesso. Id: {Id}", ciclo.Id);

            return ResultadoOperacao<CicloResponse>.Sucesso(new CicloResponse
            {
                Id = ciclo.Id,
                Nome = ciclo.Nome,
                DataInicio = ciclo.DataInicio,
                DataFim = ciclo.DataFim,
                DataCriacao = ciclo.DataCriacao,
                UltimaAtualizacao = ciclo.UltimaAtualizacao
            });
        }
    }
}
