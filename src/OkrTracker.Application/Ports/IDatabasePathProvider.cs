namespace OkrTracker.Application.Ports
{
    /// <summary>
    /// Porta de saída para gerenciar o caminho do arquivo de banco de dados LiteDB.
    /// </summary>
    public interface IDatabasePathProvider
    {
        /// <summary>
        /// Obtém o caminho atual do arquivo .db configurado.
        /// Retorna null se ainda não foi configurado.
        /// </summary>
        string? ObterCaminho();

        /// <summary>
        /// Define o caminho do arquivo .db.
        /// </summary>
        void DefinirCaminho(string caminho);

        /// <summary>
        /// Verifica se o caminho do banco de dados já foi configurado.
        /// </summary>
        bool EstaConfigurado();

        /// <summary>
        /// Remove o caminho configurado, desconectando a base de dados.
        /// </summary>
        void LimparCaminho();
    }
}
