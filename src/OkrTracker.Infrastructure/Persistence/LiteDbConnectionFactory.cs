using LiteDB;
using OkrTracker.Application.Ports;
using OkrTracker.Domain.Enums;

namespace OkrTracker.Infrastructure.Persistence
{
    /// <summary>
    /// Factory que fornece instâncias de LiteDatabase com base no caminho configurado.
    /// Usado pelos repositórios para acessar o banco.
    /// </summary>
    public class LiteDbConnectionFactory
    {
        private readonly IDatabasePathProvider _pathProvider;
        private static bool _mapperConfigurado;
        private static readonly object _lock = new();

        public LiteDbConnectionFactory(IDatabasePathProvider pathProvider)
        {
            _pathProvider = pathProvider;
            ConfigurarMapper();
        }

        /// <summary>
        /// Configura o BsonMapper global para lidar com valores legados de enums.
        /// </summary>
        private static void ConfigurarMapper()
        {
            lock (_lock)
            {
                if (_mapperConfigurado) return;

                BsonMapper.Global.RegisterType<Status>(
                    serialize: status => status.ToString(),
                    deserialize: bson =>
                    {
                        var valor = bson.AsString;
                        if (valor == "EmAndamentoAvancado")
                            return Status.EmAndamento;
                        return Enum.Parse<Status>(valor);
                    });

                _mapperConfigurado = true;
            }
        }

        /// <summary>
        /// Cria uma nova instância de LiteDatabase.
        /// O chamador é responsável por fazer o Dispose.
        /// </summary>
        public LiteDatabase CriarConexao()
        {
            var caminho = _pathProvider.ObterCaminho()
                ?? throw new InvalidOperationException("O caminho da base de dados não foi configurado.");

            // Shared mode evita lock exclusivo do arquivo quando existe mais de um processo acessando a base.
            var connectionString = new ConnectionString
            {
                Filename = caminho,
                Connection = ConnectionType.Shared
            };

            return new LiteDatabase(connectionString);
        }
    }
}
