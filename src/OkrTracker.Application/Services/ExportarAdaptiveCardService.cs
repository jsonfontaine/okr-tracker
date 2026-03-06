using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por exportar OKRs no formato Adaptive Card (JSON).
    /// Gera a estrutura compatível com Adaptive Card schema 1.5 para uso no Outlook.
    /// </summary>
    public class ExportarAdaptiveCardService : IExportarAdaptiveCardService
    {
        private readonly IObjetivoRepository _objetivoRepository;
        private readonly IKeyResultRepository _krRepository;
        private readonly IFatoRelevanteRepository _fatoRelevanteRepository;
        private readonly IRiscoRepository _riscoRepository;
        private readonly ILogger<ExportarAdaptiveCardService> _logger;

        public ExportarAdaptiveCardService(
            IObjetivoRepository objetivoRepository,
            IKeyResultRepository krRepository,
            IFatoRelevanteRepository fatoRelevanteRepository,
            IRiscoRepository riscoRepository,
            ILogger<ExportarAdaptiveCardService> logger)
        {
            _objetivoRepository = objetivoRepository;
            _krRepository = krRepository;
            _fatoRelevanteRepository = fatoRelevanteRepository;
            _riscoRepository = riscoRepository;
            _logger = logger;
        }

        public ResultadoOperacao<object> Executar(string cicloId, string timeId)
        {
            _logger.LogInformation("Exportando Adaptive Card para ciclo {CicloId} e time {TimeId}.", cicloId, timeId);

            if (string.IsNullOrWhiteSpace(cicloId))
                return ResultadoOperacao<object>.Erro("O cicloId é obrigatório.");

            if (string.IsNullOrWhiteSpace(timeId))
                return ResultadoOperacao<object>.Erro("O timeId é obrigatório.");

            var objetivos = _objetivoRepository.ObterPorCicloETime(cicloId, timeId).ToList();

            var bodyElements = new List<object>();

            // Título do card
            bodyElements.Add(new
            {
                type = "TextBlock",
                text = "OKR Tracker — Resumo",
                weight = "Bolder",
                size = "Large",
                wrap = true
            });

            if (objetivos.Count == 0)
            {
                bodyElements.Add(new
                {
                    type = "TextBlock",
                    text = "Não há OKRs cadastrados para este time/ciclo.",
                    wrap = true,
                    isSubtle = true
                });
            }
            else
            {
                foreach (var obj in objetivos)
                {
                    // Separador
                    bodyElements.Add(new { type = "TextBlock", text = "---", separator = true });

                    // Objetivo
                    bodyElements.Add(new
                    {
                        type = "TextBlock",
                        text = $"🎯 {obj.Titulo} — {obj.Progresso:F0}% ({obj.Status})",
                        weight = "Bolder",
                        size = "Medium",
                        wrap = true
                    });

                    bodyElements.Add(new
                    {
                        type = "TextBlock",
                        text = $"Prioridade: {obj.Prioridade} | Farol: {obj.Farol}",
                        isSubtle = true,
                        wrap = true
                    });

                    if (!string.IsNullOrWhiteSpace(obj.Valor))
                    {
                        bodyElements.Add(new
                        {
                            type = "TextBlock",
                            text = $"💎 Valor: {obj.Valor}",
                            isSubtle = true,
                            wrap = true
                        });
                    }

                    // KRs do objetivo
                    var krs = _krRepository.ObterPorObjetivoId(obj.Id);
                    foreach (var kr in krs)
                    {
                        bodyElements.Add(new
                        {
                            type = "TextBlock",
                            text = $"  📌 {kr.Titulo} [{kr.Tipo}] — {kr.Progresso:F0}% ({kr.Status})",
                            wrap = true
                        });
                    }

                    // Fatos relevantes do objetivo
                    var fatos = _fatoRelevanteRepository.ObterPorObjetivoId(obj.Id).ToList();
                    if (fatos.Count > 0)
                    {
                        bodyElements.Add(new
                        {
                            type = "TextBlock",
                            text = "  📝 Fatos Relevantes:",
                            weight = "Bolder",
                            wrap = true
                        });
                        foreach (var f in fatos)
                        {
                            bodyElements.Add(new
                            {
                                type = "TextBlock",
                                text = $"    • {f.Texto}",
                                wrap = true
                            });
                        }
                    }

                    // Riscos do objetivo
                    var riscos = _riscoRepository.ObterPorObjetivoId(obj.Id).ToList();
                    if (riscos.Count > 0)
                    {
                        bodyElements.Add(new
                        {
                            type = "TextBlock",
                            text = "  ⚠️ Riscos:",
                            weight = "Bolder",
                            wrap = true
                        });
                        foreach (var r in riscos)
                        {
                            var impactoTexto = string.IsNullOrWhiteSpace(r.Impacto) ? "" : $" (Impacto: {r.Impacto})";
                            bodyElements.Add(new
                            {
                                type = "TextBlock",
                                text = $"    • {r.Descricao}{impactoTexto}",
                                wrap = true
                            });
                        }
                    }
                }
            }

            var adaptiveCard = new
            {
                type = "AdaptiveCard",
                version = "1.5",
                body = bodyElements
            };

            return ResultadoOperacao<object>.Sucesso(adaptiveCard);
        }
    }
}
