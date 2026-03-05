using OkrTracker.Application.Ports;

namespace OkrTracker.Infrastructure.Persistence
{
    /// <summary>
    /// Implementação em memória do provider de caminho da base de dados LiteDB.
    /// Armazena o caminho configurado pelo usuário em tempo de execução.
    /// </summary>
    public class DatabasePathProvider : IDatabasePathProvider
    {
        private string? _caminho;

        public string? ObterCaminho() => _caminho;

        public void DefinirCaminho(string caminho) => _caminho = caminho;

        public bool EstaConfigurado() => !string.IsNullOrWhiteSpace(_caminho);
    }
}
