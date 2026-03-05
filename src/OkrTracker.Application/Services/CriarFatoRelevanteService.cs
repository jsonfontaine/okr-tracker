using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por registrar um fato relevante em um objetivo ou KR.
    /// Exatamente um entre ObjetivoId e KrId deve ser preenchido.
    /// </summary>
    public class CriarFatoRelevanteService : ICriarFatoRelevanteService
    {
        private readonly IFatoRelevanteRepository _fatoRelevanteRepository;
        private readonly IObjetivoRepository _objetivoRepository;
        private readonly IKeyResultRepository _krRepository;
        private readonly ILogger<CriarFatoRelevanteService> _logger;

        public CriarFatoRelevanteService(
            IFatoRelevanteRepository fatoRelevanteRepository,
            IObjetivoRepository objetivoRepository,
            IKeyResultRepository krRepository,
            ILogger<CriarFatoRelevanteService> logger)
        {
            _fatoRelevanteRepository = fatoRelevanteRepository;
            _objetivoRepository = objetivoRepository;
            _krRepository = krRepository;
            _logger = logger;
        }

        public ResultadoOperacao<FatoRelevanteResponse> Executar(CriarFatoRelevanteRequest request)
        {
            _logger.LogInformation("Criando fato relevante.");

            if (string.IsNullOrWhiteSpace(request.Texto))
                return ResultadoOperacao<FatoRelevanteResponse>.Erro("Texto do fato relevante é obrigatório.");

            var temObjetivo = !string.IsNullOrWhiteSpace(request.ObjetivoId);
            var temKr = !string.IsNullOrWhiteSpace(request.KrId);

            if (temObjetivo == temKr)
                return ResultadoOperacao<FatoRelevanteResponse>.Erro("Informe exatamente um entre ObjetivoId e KrId.");

            if (temObjetivo)
            {
                var objetivo = _objetivoRepository.ObterPorId(request.ObjetivoId!);
                if (objetivo == null)
                    return ResultadoOperacao<FatoRelevanteResponse>.Erro("Objetivo não encontrado.");
            }

            if (temKr)
            {
                var kr = _krRepository.ObterPorId(request.KrId!);
                if (kr == null)
                    return ResultadoOperacao<FatoRelevanteResponse>.Erro("KR não encontrado.");
            }

            var fato = new FatoRelevante
            {
                Id = Guid.NewGuid().ToString(),
                Texto = request.Texto,
                ObjetivoId = temObjetivo ? request.ObjetivoId : null,
                KrId = temKr ? request.KrId : null,
                DataCriacao = DateTime.UtcNow
            };

            _fatoRelevanteRepository.Inserir(fato);
            _logger.LogInformation("Fato relevante criado com sucesso. Id: {Id}", fato.Id);

            return ResultadoOperacao<FatoRelevanteResponse>.Sucesso(new FatoRelevanteResponse
            {
                Id = fato.Id,
                Texto = fato.Texto,
                ObjetivoId = fato.ObjetivoId,
                KrId = fato.KrId,
                DataCriacao = fato.DataCriacao
            });
        }
    }
}
