using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para excluir um Key Result.
    /// Impede exclusão se for o último KR do objetivo.
    /// </summary>
    public interface IExcluirKRService
    {
        ResultadoOperacao Executar(string id);
    }
}
