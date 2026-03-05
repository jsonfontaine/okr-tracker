using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por criar um novo time.
    /// Valida nome obrigatório e unicidade.
    /// </summary>
    public class CriarTimeService : ICriarTimeService
    {
        private readonly ITimeRepository _timeRepository;
        private readonly ILogger<CriarTimeService> _logger;

        public CriarTimeService(ITimeRepository timeRepository, ILogger<CriarTimeService> logger)
        {
            _timeRepository = timeRepository;
            _logger = logger;
        }

        public ResultadoOperacao<TimeResponse> Executar(CriarTimeRequest request)
        {
            _logger.LogInformation("Criando time com nome: {Nome}", request.Nome);

            if (string.IsNullOrWhiteSpace(request.Nome))
                return ResultadoOperacao<TimeResponse>.Erro("O nome do time é obrigatório.");

            var existente = _timeRepository.ObterPorNome(request.Nome);
            if (existente != null)
                return ResultadoOperacao<TimeResponse>.Erro("Já existe um time com este nome.");

            var agora = DateTime.UtcNow;
            var time = new Time
            {
                Id = Guid.NewGuid().ToString(),
                Nome = request.Nome,
                Descricao = request.Descricao,
                DataCriacao = agora,
                UltimaAtualizacao = agora
            };

            _timeRepository.Inserir(time);
            _logger.LogInformation("Time criado com sucesso. Id: {Id}", time.Id);

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
