namespace OkrTracker.Application.Ports
{
    /// <summary>
    /// Porta de saída para validar se o LiteDB consegue abrir um arquivo .db no caminho especificado.
    /// </summary>
    public interface IDatabaseValidator
    {
        /// <summary>
        /// Tenta abrir o arquivo .db no caminho especificado.
        /// Retorna true se conseguiu abrir com sucesso.
        /// </summary>
        bool Validar(string databasePath);
    }
}
