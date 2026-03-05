using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Enums;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por atualizar campos de um objetivo existente.
    /// Valida existência, campos obrigatórios e referências de ciclo e time.
    /// </summary>
    public class AtualizarObjetivoService : IAtualizarObjetivoService
    {
        private readonly IObjetivoRepository _objetivoRepository;
        private readonly ICicloRepository _cicloRepository;
        private readonly ITimeRepository _timeRepository;
        private readonly ILogger<AtualizarObjetivoService> _logger;

        public AtualizarObjetivoService(
            IObjetivoRepository objetivoRepository,
            ICicloRepository cicloRepository,
            ITimeRepository timeRepository,
            ILogger<AtualizarObjetivoService> logger)
        {
            _objetivoRepository = objetivoRepository;
            _cicloRepository = cicloRepository;
            _timeRepository = timeRepository;
            _logger = logger;
        }

        public ResultadoOperacao<ObjetivoResponse> Executar(string id, AtualizarObjetivoRequest request)
        {
            _logger.LogInformation("Atualizando objetivo {Id}.", id);

            if (string.IsNullOrWhiteSpace(request.Titulo))
                return ResultadoOperacao<ObjetivoResponse>.Erro("Título do objetivo é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.Descricao))
                return ResultadoOperacao<ObjetivoResponse>.Erro("Descrição do objetivo é obrigatória.");

            if (string.IsNullOrWhiteSpace(request.CicloId))
                return ResultadoOperacao<ObjetivoResponse>.Erro("O ciclo é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.TimeId))
                return ResultadoOperacao<ObjetivoResponse>.Erro("O time é obrigatório.");

            var objetivo = _objetivoRepository.ObterPorId(id);
            if (objetivo == null)
                return ResultadoOperacao<ObjetivoResponse>.Erro("Objetivo não encontrado.");

            var ciclo = _cicloRepository.ObterPorId(request.CicloId);
            if (ciclo == null)
                return ResultadoOperacao<ObjetivoResponse>.Erro("Ciclo não encontrado.");

            var time = _timeRepository.ObterPorId(request.TimeId);
            if (time == null)
                return ResultadoOperacao<ObjetivoResponse>.Erro("Time não encontrado.");

            if (!Enum.TryParse<Prioridade>(request.Prioridade, true, out var prioridade))
                prioridade = Prioridade.Media;

            if (!Enum.TryParse<Farol>(request.Farol, true, out var farol))
                farol = Farol.Verde;

            objetivo.Titulo = request.Titulo;
            objetivo.Descricao = request.Descricao;
            objetivo.CicloId = request.CicloId;
            objetivo.TimeId = request.TimeId;
            objetivo.Prioridade = prioridade;
            objetivo.Farol = farol;
            objetivo.Intruder = request.Intruder;
            objetivo.DescobertaTardia = request.DescobertaTardia;
            objetivo.UltimaAtualizacao = DateTime.UtcNow;

            _objetivoRepository.Atualizar(objetivo);
            _logger.LogInformation("Objetivo {Id} atualizado com sucesso.", id);

            return ResultadoOperacao<ObjetivoResponse>.Sucesso(new ObjetivoResponse
            {
                Id = objetivo.Id,
                Titulo = objetivo.Titulo,
                Descricao = objetivo.Descricao,
                CicloId = objetivo.CicloId,
                TimeId = objetivo.TimeId,
                Prioridade = objetivo.Prioridade.ToString(),
                Progresso = objetivo.Progresso,
                Status = objetivo.Status.ToString(),
                Farol = objetivo.Farol.ToString(),
                Intruder = objetivo.Intruder,
                DescobertaTardia = objetivo.DescobertaTardia,
                DataCriacao = objetivo.DataCriacao,
                UltimaAtualizacao = objetivo.UltimaAtualizacao
            });
        }
    }
}
