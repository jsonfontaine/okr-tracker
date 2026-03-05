using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para registrar um risco em um objetivo ou KR.
    /// </summary>
    public interface ICriarRiscoService
    {
        ResultadoOperacao<RiscoResponse> Executar(CriarRiscoRequest request);
    }
}
