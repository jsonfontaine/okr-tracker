using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para configurar o caminho da base de dados LiteDB.
    /// </summary>
    public interface IConfigurarBaseDeDadosService
    {
        /// <summary>
        /// Configura o caminho do arquivo .db e valida se o LiteDB consegue abrir.
        /// </summary>
        ResultadoOperacao Configurar(string databasePath);
    }
}
