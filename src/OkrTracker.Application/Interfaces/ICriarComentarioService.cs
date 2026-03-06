using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para registrar um comentário em um objetivo ou KR.
    /// </summary>
    public interface ICriarComentarioService
    {
        ResultadoOperacao<ComentarioResponse> Executar(CriarComentarioRequest request);
    }
}
