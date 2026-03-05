using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Enums;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por criar um novo objetivo vinculado a um ciclo e time.
    /// Valida campos obrigatórios e existência de ciclo e time.
    /// </summary>
    public class CriarObjetivoService : ICriarObjetivoService
    {
        private readonly IObjetivoRepository _objetivoRepository;
        private readonly ICicloRepository _cicloRepository;
        private readonly ITimeRepository _timeRepository;
        private readonly ILogger<CriarObjetivoService> _logger;

        public CriarObjetivoService(
            IObjetivoRepository objetivoRepository,
            ICicloRepository cicloRepository,
            ITimeRepository timeRepository,
            ILogger<CriarObjetivoService> logger)
        {
            _objetivoRepository = objetivoRepository;
            _cicloRepository = cicloRepository;
            _timeRepository = timeRepository;
            _logger = logger;
        }

        public ResultadoOperacao<ObjetivoResponse> Executar(CriarObjetivoRequest request)
        {
            _logger.LogInformation("Criando objetivo: {Titulo}", request.Titulo);

            if (string.IsNullOrWhiteSpace(request.Titulo))
                return ResultadoOperacao<ObjetivoResponse>.Erro("Título do objetivo é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.Descricao))
                return ResultadoOperacao<ObjetivoResponse>.Erro("Descrição do objetivo é obrigatória.");

            if (string.IsNullOrWhiteSpace(request.CicloId))
                return ResultadoOperacao<ObjetivoResponse>.Erro("O ciclo é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.TimeId))
                return ResultadoOperacao<ObjetivoResponse>.Erro("O time é obrigatório.");

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

            var agora = DateTime.UtcNow;
            var objetivo = new Objetivo
            {
                Id = Guid.NewGuid().ToString(),
                Titulo = request.Titulo,
                Descricao = request.Descricao,
                CicloId = request.CicloId,
                TimeId = request.TimeId,
                Prioridade = prioridade,
                Progresso = 0,
                Status = Status.NaoIniciado,
                Farol = farol,
                Intruder = request.Intruder,
                DescobertaTardia = request.DescobertaTardia,
                DataCriacao = agora,
                UltimaAtualizacao = agora
            };

            _objetivoRepository.Inserir(objetivo);
            _logger.LogInformation("Objetivo criado com sucesso. Id: {Id}", objetivo.Id);

            return ResultadoOperacao<ObjetivoResponse>.Sucesso(MapearObjetivoResponse(objetivo));
        }

        private static ObjetivoResponse MapearObjetivoResponse(Objetivo obj)
        {
            return new ObjetivoResponse
            {
                Id = obj.Id,
                Titulo = obj.Titulo,
                Descricao = obj.Descricao,
                CicloId = obj.CicloId,
                TimeId = obj.TimeId,
                Prioridade = obj.Prioridade.ToString(),
                Progresso = obj.Progresso,
                Status = obj.Status.ToString(),
                Farol = obj.Farol.ToString(),
                Intruder = obj.Intruder,
                DescobertaTardia = obj.DescobertaTardia,
                DataCriacao = obj.DataCriacao,
                UltimaAtualizacao = obj.UltimaAtualizacao
            };
        }
    }
}
