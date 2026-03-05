using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para listar todos os times.
    /// </summary>
    public interface IListarTimesService
    {
        ResultadoOperacao<IEnumerable<TimeResponse>> Executar();
    }
}
