using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por atualizar o nome de um ciclo existente.
    /// Valida existência, nome obrigatório e unicidade.
    /// </summary>
    public class AtualizarCicloService : IAtualizarCicloService
    {
        private readonly ICicloRepository _cicloRepository;
        private readonly ILogger<AtualizarCicloService> _logger;

        public AtualizarCicloService(ICicloRepository cicloRepository, ILogger<AtualizarCicloService> logger)
        {
            _cicloRepository = cicloRepository;
            _logger = logger;
        }

        public ResultadoOperacao<CicloResponse> Executar(string id, AtualizarCicloRequest request)
        {
            _logger.LogInformation("Atualizando ciclo {Id} para nome: {Nome}", id, request.Nome);

            if (string.IsNullOrWhiteSpace(request.Nome))
                return ResultadoOperacao<CicloResponse>.Erro("O nome do ciclo é obrigatório.");

            // Validar consistência de datas se informadas
            if (request.DataInicio.HasValue && request.DataFim.HasValue)
            {
                if (request.DataInicio.Value > request.DataFim.Value)
                    return ResultadoOperacao<CicloResponse>.Erro("A data de início não pode ser posterior à data de término.");
            }

            var ciclo = _cicloRepository.ObterPorId(id);
            if (ciclo == null)
                return ResultadoOperacao<CicloResponse>.Erro("Ciclo não encontrado.");

            var existente = _cicloRepository.ObterPorNome(request.Nome);
            if (existente != null && existente.Id != id)
                return ResultadoOperacao<CicloResponse>.Erro("Já existe um ciclo com este nome.");

            ciclo.Nome = request.Nome;
            ciclo.DataInicio = request.DataInicio;
            ciclo.DataFim = request.DataFim;
            ciclo.UltimaAtualizacao = DateTime.UtcNow;

            _cicloRepository.Atualizar(ciclo);
            _logger.LogInformation("Ciclo {Id} atualizado com sucesso.", id);

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
