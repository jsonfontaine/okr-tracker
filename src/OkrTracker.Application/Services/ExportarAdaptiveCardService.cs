using System.Text;
using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por gerar o resumo executivo dos OKRs em texto formatado,
    /// pronto para copiar e colar no Outlook.
    /// </summary>
    public class ExportarResumoExecutivoService : IExportarResumoExecutivoService
    {
        private readonly IObjetivoRepository _objetivoRepository;
        private readonly IKeyResultRepository _krRepository;
        private readonly IFatoRelevanteRepository _fatoRelevanteRepository;
        private readonly IRiscoRepository _riscoRepository;
        private readonly ICicloRepository _cicloRepository;
        private readonly IProjetoRepository _projetoRepository;
        private readonly ILogger<ExportarResumoExecutivoService> _logger;

        public ExportarResumoExecutivoService(
            IObjetivoRepository objetivoRepository,
            IKeyResultRepository krRepository,
            IFatoRelevanteRepository fatoRelevanteRepository,
            IRiscoRepository riscoRepository,
            ICicloRepository cicloRepository,
            IProjetoRepository projetoRepository,
            ILogger<ExportarResumoExecutivoService> logger)
        {
            _objetivoRepository = objetivoRepository;
            _krRepository = krRepository;
            _fatoRelevanteRepository = fatoRelevanteRepository;
            _riscoRepository = riscoRepository;
            _cicloRepository = cicloRepository;
            _projetoRepository = projetoRepository;
            _logger = logger;
        }

        public ResultadoOperacao<string> Executar(string cicloId, string projetoId)
        {
            _logger.LogInformation("Gerando resumo executivo para ciclo {CicloId} e projeto {ProjetoId}.", cicloId, projetoId);

            if (string.IsNullOrWhiteSpace(cicloId))
                return ResultadoOperacao<string>.Erro("O cicloId é obrigatório.");

            if (string.IsNullOrWhiteSpace(projetoId))
                return ResultadoOperacao<string>.Erro("O projetoId é obrigatório.");

            var ciclo = _cicloRepository.ObterPorId(cicloId);
            var projeto = _projetoRepository.ObterPorId(projetoId);
            var nomeCiclo = ciclo?.Nome ?? cicloId;
            var nomeProjeto = projeto?.Nome ?? projetoId;

            var objetivos = _objetivoRepository.ObterPorCicloEProjeto(cicloId, projetoId).ToList();

            var sb = new StringBuilder();

            sb.AppendLine("═══════════════════════════════════════════");
            sb.AppendLine($"  📊 RESUMO EXECUTIVO — {nomeProjeto} | {nomeCiclo}");
            sb.AppendLine("═══════════════════════════════════════════");
            sb.AppendLine();

            if (objetivos.Count == 0)
            {
                sb.AppendLine("Não há OKRs cadastrados para este projeto/ciclo.");
                return ResultadoOperacao<string>.Sucesso(sb.ToString());
            }

            for (var i = 0; i < objetivos.Count; i++)
            {
                var obj = objetivos[i];

                sb.AppendLine("───────────────────────────────────────────");
                sb.AppendLine($"🎯 OBJETIVO {i + 1}: {obj.Titulo}");
                sb.AppendLine($"   Progresso: {obj.Progresso:F0}% | Status: {obj.Status} | Prioridade: {obj.Prioridade} | Farol: {EmojiFarol(obj.Farol.ToString())} {obj.Farol}");

                if (!string.IsNullOrWhiteSpace(obj.Valor))
                    sb.AppendLine($"   💎 Valor: {obj.Valor}");

                sb.AppendLine();

                // KRs
                var krs = _krRepository.ObterPorObjetivoId(obj.Id).ToList();
                if (krs.Count > 0)
                {
                    sb.AppendLine("   Key Results:");
                    foreach (var kr in krs)
                    {
                        sb.AppendLine($"   📌 {kr.Titulo} [{kr.Tipo}] — {kr.Progresso:F0}% ({kr.Status}) | Farol: {EmojiFarol(kr.Farol.ToString())} {kr.Farol}");
                    }
                    sb.AppendLine();
                }

                // Fatos Relevantes
                var fatos = _fatoRelevanteRepository.ObterPorObjetivoId(obj.Id).ToList();
                if (fatos.Count > 0)
                {
                    sb.AppendLine("   📝 Fatos Relevantes:");
                    foreach (var f in fatos)
                        sb.AppendLine($"   • {f.Texto}");
                    sb.AppendLine();
                }

                // Riscos
                var riscos = _riscoRepository.ObterPorObjetivoId(obj.Id).ToList();
                if (riscos.Count > 0)
                {
                    sb.AppendLine("   ⚠️ Riscos:");
                    foreach (var r in riscos)
                    {
                        var impacto = string.IsNullOrWhiteSpace(r.Impacto) ? "" : $" (Impacto: {r.Impacto})";
                        sb.AppendLine($"   • {r.Descricao}{impacto}");
                    }
                    sb.AppendLine();
                }
            }

            sb.AppendLine("═══════════════════════════════════════════");
            sb.AppendLine($"Gerado em: {DateTime.UtcNow:dd/MM/yyyy HH:mm} UTC");

            return ResultadoOperacao<string>.Sucesso(sb.ToString());
        }

        private static string EmojiFarol(string farol) => farol switch
        {
            "Verde" => "✅",
            "Amarelo" => "⚠️",
            "Vermelho" => "🔴",
            _ => "⚪"
        };
    }
}
