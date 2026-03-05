using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para criar um novo time.
    /// </summary>
    public interface ICriarTimeService
    {
        ResultadoOperacao<TimeResponse> Executar(CriarTimeRequest request);
    }
}
