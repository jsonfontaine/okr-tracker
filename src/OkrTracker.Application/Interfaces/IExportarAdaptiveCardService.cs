using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para exportar OKRs no formato Adaptive Card (JSON).
    /// Gera a estrutura compatível com Adaptive Card schema 1.5 para uso no Outlook.
    /// </summary>
    public interface IExportarAdaptiveCardService
    {
        ResultadoOperacao<object> Executar(string cicloId, string timeId);
    }
}
