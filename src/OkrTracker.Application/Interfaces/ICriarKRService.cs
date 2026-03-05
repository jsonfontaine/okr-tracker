using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para criar um Key Result vinculado a um objetivo.
    /// </summary>
    public interface ICriarKRService
    {
        ResultadoOperacao<KeyResultResponse> Executar(CriarKeyResultRequest request);
    }
}
