using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para registrar um fato relevante em um objetivo ou KR.
    /// </summary>
    public interface ICriarFatoRelevanteService
    {
        ResultadoOperacao<FatoRelevanteResponse> Executar(CriarFatoRelevanteRequest request);
    }
}
