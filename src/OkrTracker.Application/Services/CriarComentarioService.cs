using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por registrar um comentário (check-in) em um objetivo ou KR.
    /// Exatamente um entre ObjetivoId e KrId deve ser preenchido.
    /// </summary>
    public class CriarComentarioService : ICriarComentarioService
    {
        private readonly IComentarioRepository _comentarioRepository;
        private readonly IObjetivoRepository _objetivoRepository;
        private readonly IKeyResultRepository _krRepository;
        private readonly ILogger<CriarComentarioService> _logger;

        public CriarComentarioService(
            IComentarioRepository comentarioRepository,
            IObjetivoRepository objetivoRepository,
            IKeyResultRepository krRepository,
            ILogger<CriarComentarioService> logger)
        {
            _comentarioRepository = comentarioRepository;
            _objetivoRepository = objetivoRepository;
            _krRepository = krRepository;
            _logger = logger;
        }

        public ResultadoOperacao<ComentarioResponse> Executar(CriarComentarioRequest request)
        {
            _logger.LogInformation("Criando comentário.");

            if (string.IsNullOrWhiteSpace(request.Texto))
                return ResultadoOperacao<ComentarioResponse>.Erro("Texto do check-in é obrigatório.");

            // Exatamente um entre ObjetivoId e KrId deve ser preenchido
            var temObjetivo = !string.IsNullOrWhiteSpace(request.ObjetivoId);
            var temKr = !string.IsNullOrWhiteSpace(request.KrId);

            if (temObjetivo == temKr)
                return ResultadoOperacao<ComentarioResponse>.Erro("Informe exatamente um entre ObjetivoId e KrId.");

            if (temObjetivo)
            {
                var objetivo = _objetivoRepository.ObterPorId(request.ObjetivoId!);
                if (objetivo == null)
                    return ResultadoOperacao<ComentarioResponse>.Erro("Objetivo não encontrado.");
            }

            if (temKr)
            {
                var kr = _krRepository.ObterPorId(request.KrId!);
                if (kr == null)
                    return ResultadoOperacao<ComentarioResponse>.Erro("KR não encontrado.");
            }

            var comentario = new Comentario
            {
                Id = Guid.NewGuid().ToString(),
                Texto = request.Texto,
                ObjetivoId = temObjetivo ? request.ObjetivoId : null,
                KrId = temKr ? request.KrId : null,
                DataCriacao = DateTime.UtcNow
            };

            _comentarioRepository.Inserir(comentario);
            _logger.LogInformation("Comentário criado com sucesso. Id: {Id}", comentario.Id);

            return ResultadoOperacao<ComentarioResponse>.Sucesso(new ComentarioResponse
            {
                Id = comentario.Id,
                Texto = comentario.Texto,
                ObjetivoId = comentario.ObjetivoId,
                KrId = comentario.KrId,
                DataCriacao = comentario.DataCriacao
            });
        }
    }
}
