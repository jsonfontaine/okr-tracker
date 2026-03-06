using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por listar OKRs filtrados por ciclo e time.
    /// Retorna objetivos com seus KRs, comentários, fatos relevantes e riscos.
    /// </summary>
    public class ListarOKRsPorTimeECicloService : IListarOKRsPorTimeECicloService
    {
        private readonly IObjetivoRepository _objetivoRepository;
        private readonly IKeyResultRepository _krRepository;
        private readonly IComentarioRepository _comentarioRepository;
        private readonly IFatoRelevanteRepository _fatoRelevanteRepository;
        private readonly IRiscoRepository _riscoRepository;
        private readonly ILogger<ListarOKRsPorTimeECicloService> _logger;

        public ListarOKRsPorTimeECicloService(
            IObjetivoRepository objetivoRepository,
            IKeyResultRepository krRepository,
            IComentarioRepository comentarioRepository,
            IFatoRelevanteRepository fatoRelevanteRepository,
            IRiscoRepository riscoRepository,
            ILogger<ListarOKRsPorTimeECicloService> logger)
        {
            _objetivoRepository = objetivoRepository;
            _krRepository = krRepository;
            _comentarioRepository = comentarioRepository;
            _fatoRelevanteRepository = fatoRelevanteRepository;
            _riscoRepository = riscoRepository;
            _logger = logger;
        }

        public ResultadoOperacao<IEnumerable<ObjetivoResponse>> Executar(string cicloId, string timeId)
        {
            _logger.LogInformation("Listando OKRs para ciclo {CicloId} e time {TimeId}.", cicloId, timeId);

            if (string.IsNullOrWhiteSpace(cicloId))
                return ResultadoOperacao<IEnumerable<ObjetivoResponse>>.Erro("O cicloId é obrigatório.");

            if (string.IsNullOrWhiteSpace(timeId))
                return ResultadoOperacao<IEnumerable<ObjetivoResponse>>.Erro("O timeId é obrigatório.");

            var objetivos = _objetivoRepository.ObterPorCicloETime(cicloId, timeId);
            var response = new List<ObjetivoResponse>();

            foreach (var obj in objetivos)
            {
                var objetivoResponse = new ObjetivoResponse
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
                    Valor = obj.Valor,
                    DataCriacao = obj.DataCriacao,
                    UltimaAtualizacao = obj.UltimaAtualizacao
                };

                // KRs do objetivo
                var krs = _krRepository.ObterPorObjetivoId(obj.Id);
                foreach (var kr in krs)
                {
                    var krResponse = new KeyResultResponse
                    {
                        Id = kr.Id,
                        ObjetivoId = kr.ObjetivoId,
                        Titulo = kr.Titulo,
                        Descricao = kr.Descricao,
                        Tipo = kr.Tipo.ToString(),
                        Progresso = kr.Progresso,
                        Status = kr.Status.ToString(),
                        Farol = kr.Farol.ToString(),
                        Intruder = kr.Intruder,
                        DescobertaTardia = kr.DescobertaTardia,
                        DataCriacao = kr.DataCriacao,
                        UltimaAtualizacao = kr.UltimaAtualizacao
                    };

                    // Comentários do KR
                    krResponse.Comentarios = _comentarioRepository.ObterPorKrId(kr.Id)
                        .Select(c => new ComentarioResponse
                        {
                            Id = c.Id,
                            Texto = c.Texto,
                            ObjetivoId = c.ObjetivoId,
                            KrId = c.KrId,
                            DataCriacao = c.DataCriacao
                        }).ToList();

                    // Fatos relevantes do KR
                    krResponse.FatosRelevantes = _fatoRelevanteRepository.ObterPorKrId(kr.Id)
                        .Select(f => new FatoRelevanteResponse
                        {
                            Id = f.Id,
                            Texto = f.Texto,
                            ObjetivoId = f.ObjetivoId,
                            KrId = f.KrId,
                            DataCriacao = f.DataCriacao
                        }).ToList();

                    // Riscos do KR
                    krResponse.Riscos = _riscoRepository.ObterPorKrId(kr.Id)
                        .Select(r => new RiscoResponse
                        {
                            Id = r.Id,
                            Descricao = r.Descricao,
                            Impacto = r.Impacto,
                            ObjetivoId = r.ObjetivoId,
                            KrId = r.KrId,
                            DataCriacao = r.DataCriacao
                        }).ToList();

                    objetivoResponse.KeyResults.Add(krResponse);
                }

                // Comentários do objetivo
                objetivoResponse.Comentarios = _comentarioRepository.ObterPorObjetivoId(obj.Id)
                    .Select(c => new ComentarioResponse
                    {
                        Id = c.Id,
                        Texto = c.Texto,
                        ObjetivoId = c.ObjetivoId,
                        KrId = c.KrId,
                        DataCriacao = c.DataCriacao
                    }).ToList();

                // Fatos relevantes do objetivo
                objetivoResponse.FatosRelevantes = _fatoRelevanteRepository.ObterPorObjetivoId(obj.Id)
                    .Select(f => new FatoRelevanteResponse
                    {
                        Id = f.Id,
                        Texto = f.Texto,
                        ObjetivoId = f.ObjetivoId,
                        KrId = f.KrId,
                        DataCriacao = f.DataCriacao
                    }).ToList();

                // Riscos do objetivo
                objetivoResponse.Riscos = _riscoRepository.ObterPorObjetivoId(obj.Id)
                    .Select(r => new RiscoResponse
                    {
                        Id = r.Id,
                        Descricao = r.Descricao,
                        Impacto = r.Impacto,
                        ObjetivoId = r.ObjetivoId,
                        KrId = r.KrId,
                        DataCriacao = r.DataCriacao
                    }).ToList();

                response.Add(objetivoResponse);
            }

            return ResultadoOperacao<IEnumerable<ObjetivoResponse>>.Sucesso(response);
        }
    }
}
