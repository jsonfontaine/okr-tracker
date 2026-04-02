using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por excluir um Objetivo e todos os seus dados associados (KRs,
    /// comentários, fatos relevantes e riscos) em cascata.
    /// </summary>
    public class ExcluirObjetivoService : IExcluirObjetivoService
    {
        private readonly IObjetivoRepository _objetivoRepository;
        private readonly IKeyResultRepository _krRepository;
        private readonly IComentarioRepository _comentarioRepository;
        private readonly IFatoRelevanteRepository _fatoRelevanteRepository;
        private readonly IRiscoRepository _riscoRepository;
        private readonly ILogger<ExcluirObjetivoService> _logger;

        public ExcluirObjetivoService(
            IObjetivoRepository objetivoRepository,
            IKeyResultRepository krRepository,
            IComentarioRepository comentarioRepository,
            IFatoRelevanteRepository fatoRelevanteRepository,
            IRiscoRepository riscoRepository,
            ILogger<ExcluirObjetivoService> logger)
        {
            _objetivoRepository = objetivoRepository;
            _krRepository = krRepository;
            _comentarioRepository = comentarioRepository;
            _fatoRelevanteRepository = fatoRelevanteRepository;
            _riscoRepository = riscoRepository;
            _logger = logger;
        }

        public ResultadoOperacao Executar(string id)
        {
            _logger.LogInformation("Tentando excluir objetivo {Id}.", id);

            var objetivo = _objetivoRepository.ObterPorId(id);
            if (objetivo == null)
                return ResultadoOperacao.Erro("Objetivo não encontrado.");

            // Excluir dados dos KRs (comentários, fatos e riscos vinculados a cada KR)
            var krs = _krRepository.ObterPorObjetivoId(id).ToList();
            foreach (var kr in krs)
            {
                _comentarioRepository.ExcluirPorKrId(kr.Id);
                _fatoRelevanteRepository.ExcluirPorKrId(kr.Id);
                _riscoRepository.ExcluirPorKrId(kr.Id);
            }

            // Excluir todos os KRs do objetivo
            _krRepository.ExcluirPorObjetivoId(id);

            // Excluir dados diretamente vinculados ao objetivo
            _comentarioRepository.ExcluirPorObjetivoId(id);
            _fatoRelevanteRepository.ExcluirPorObjetivoId(id);
            _riscoRepository.ExcluirPorObjetivoId(id);

            // Por fim, excluir o objetivo
            _objetivoRepository.Excluir(id);

            _logger.LogInformation("Objetivo {Id} e seus dados associados foram excluídos.", id);
            return ResultadoOperacao.Sucesso();
        }
    }
}

