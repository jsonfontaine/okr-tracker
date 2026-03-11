using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para gerar o resumo executivo dos OKRs de um projeto/ciclo.
    /// </summary>
    public interface IExportarResumoExecutivoService
    {
        ResultadoOperacao<string> Executar(string cicloId, string projetoId);
    }
}
