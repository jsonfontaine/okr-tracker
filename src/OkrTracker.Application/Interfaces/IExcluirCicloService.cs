using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para excluir um ciclo.
    /// Só permite exclusão se não houver objetivos associados.
    /// </summary>
    public interface IExcluirCicloService
    {
        ResultadoOperacao Executar(string id);
    }
}
