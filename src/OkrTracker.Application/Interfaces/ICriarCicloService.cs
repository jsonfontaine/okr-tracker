using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para criar um novo ciclo.
    /// </summary>
    public interface ICriarCicloService
    {
        ResultadoOperacao<CicloResponse> Executar(CriarCicloRequest request);
    }
}
