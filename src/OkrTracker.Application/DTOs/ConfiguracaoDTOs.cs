namespace OkrTracker.Application.DTOs
{
    /// <summary>
    /// DTO para requisição de configuração do caminho da base de dados.
    /// </summary>
    public class ConfigurarDatabaseRequest
    {
        public string DatabasePath { get; set; } = string.Empty;
    }
}
