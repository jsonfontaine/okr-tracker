using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para excluir um time.
    /// Só permite exclusão se não houver objetivos associados.
    /// </summary>
    public interface IExcluirTimeService
    {
        ResultadoOperacao Executar(string id);
    }
}
