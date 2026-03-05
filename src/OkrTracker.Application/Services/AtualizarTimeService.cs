using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por atualizar nome/descrição de um time existente.
    /// Valida existência, nome obrigatório e unicidade.
    /// </summary>
    public class AtualizarTimeService : IAtualizarTimeService
    {
        private readonly ITimeRepository _timeRepository;
        private readonly ILogger<AtualizarTimeService> _logger;

        public AtualizarTimeService(ITimeRepository timeRepository, ILogger<AtualizarTimeService> logger)
        {
            _timeRepository = timeRepository;
            _logger = logger;
        }

        public ResultadoOperacao<TimeResponse> Executar(string id, AtualizarTimeRequest request)
        {
            _logger.LogInformation("Atualizando time {Id} para nome: {Nome}", id, request.Nome);

            if (string.IsNullOrWhiteSpace(request.Nome))
                return ResultadoOperacao<TimeResponse>.Erro("O nome do time é obrigatório.");

            var time = _timeRepository.ObterPorId(id);
            if (time == null)
                return ResultadoOperacao<TimeResponse>.Erro("Time não encontrado.");

            var existente = _timeRepository.ObterPorNome(request.Nome);
            if (existente != null && existente.Id != id)
                return ResultadoOperacao<TimeResponse>.Erro("Já existe um time com este nome.");

            time.Nome = request.Nome;
            time.Descricao = request.Descricao;
            time.UltimaAtualizacao = DateTime.UtcNow;

            _timeRepository.Atualizar(time);
            _logger.LogInformation("Time {Id} atualizado com sucesso.", id);

            return ResultadoOperacao<TimeResponse>.Sucesso(new TimeResponse
            {
                Id = time.Id,
                Nome = time.Nome,
                Descricao = time.Descricao,
                DataCriacao = time.DataCriacao,
                UltimaAtualizacao = time.UltimaAtualizacao
            });
        }
    }
}
