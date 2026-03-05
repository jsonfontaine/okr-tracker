using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para listar todos os ciclos.
    /// </summary>
    public interface IListarCiclosService
    {
        ResultadoOperacao<IEnumerable<CicloResponse>> Executar();
    }
}
