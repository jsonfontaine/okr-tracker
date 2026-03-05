using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por registrar um risco em um objetivo ou KR.
    /// Exatamente um entre ObjetivoId e KrId deve ser preenchido.
    /// </summary>
    public class CriarRiscoService : ICriarRiscoService
    {
        private readonly IRiscoRepository _riscoRepository;
        private readonly IObjetivoRepository _objetivoRepository;
        private readonly IKeyResultRepository _krRepository;
        private readonly ILogger<CriarRiscoService> _logger;

        public CriarRiscoService(
            IRiscoRepository riscoRepository,
            IObjetivoRepository objetivoRepository,
            IKeyResultRepository krRepository,
            ILogger<CriarRiscoService> logger)
        {
            _riscoRepository = riscoRepository;
            _objetivoRepository = objetivoRepository;
            _krRepository = krRepository;
            _logger = logger;
        }

        public ResultadoOperacao<RiscoResponse> Executar(CriarRiscoRequest request)
        {
            _logger.LogInformation("Criando risco.");

            if (string.IsNullOrWhiteSpace(request.Descricao))
                return ResultadoOperacao<RiscoResponse>.Erro("Descrição do risco é obrigatória.");

            var temObjetivo = !string.IsNullOrWhiteSpace(request.ObjetivoId);
            var temKr = !string.IsNullOrWhiteSpace(request.KrId);

            if (temObjetivo == temKr)
                return ResultadoOperacao<RiscoResponse>.Erro("Informe exatamente um entre ObjetivoId e KrId.");

            if (temObjetivo)
            {
                var objetivo = _objetivoRepository.ObterPorId(request.ObjetivoId!);
                if (objetivo == null)
                    return ResultadoOperacao<RiscoResponse>.Erro("Objetivo não encontrado.");
            }

            if (temKr)
            {
                var kr = _krRepository.ObterPorId(request.KrId!);
                if (kr == null)
                    return ResultadoOperacao<RiscoResponse>.Erro("KR não encontrado.");
            }

            var risco = new Risco
            {
                Id = Guid.NewGuid().ToString(),
                Descricao = request.Descricao,
                Impacto = request.Impacto,
                ObjetivoId = temObjetivo ? request.ObjetivoId : null,
                KrId = temKr ? request.KrId : null,
                DataCriacao = DateTime.UtcNow
            };

            _riscoRepository.Inserir(risco);
            _logger.LogInformation("Risco criado com sucesso. Id: {Id}", risco.Id);

            return ResultadoOperacao<RiscoResponse>.Sucesso(new RiscoResponse
            {
                Id = risco.Id,
                Descricao = risco.Descricao,
                Impacto = risco.Impacto,
                ObjetivoId = risco.ObjetivoId,
                KrId = risco.KrId,
                DataCriacao = risco.DataCriacao
            });
        }
    }
}
