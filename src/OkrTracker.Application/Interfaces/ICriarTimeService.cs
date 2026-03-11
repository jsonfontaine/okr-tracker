using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para criar um novo projeto.
    /// </summary>
    public interface ICriarTimeService
    {
        ResultadoOperacao<ProjetoResponse> Executar(CriarProjetoRequest request);
    }
}
