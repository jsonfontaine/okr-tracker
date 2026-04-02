using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para excluir um Objetivo e todos os seus dados associados.
    /// </summary>
    public interface IExcluirObjetivoService
    {
        ResultadoOperacao Executar(string id);
    }
}

