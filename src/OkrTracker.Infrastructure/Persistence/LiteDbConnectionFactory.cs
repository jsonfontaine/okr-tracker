using LiteDB;
using OkrTracker.Application.Ports;

namespace OkrTracker.Infrastructure.Persistence
{
    /// <summary>
    /// Factory que fornece instâncias de LiteDatabase com base no caminho configurado.
    /// Usado pelos repositórios para acessar o banco.
    /// </summary>
    public class LiteDbConnectionFactory
    {
        private readonly IDatabasePathProvider _pathProvider;

        public LiteDbConnectionFactory(IDatabasePathProvider pathProvider)
        {
            _pathProvider = pathProvider;
        }

        /// <summary>
        /// Cria uma nova instância de LiteDatabase.
        /// O chamador é responsável por fazer o Dispose.
        /// </summary>
        public LiteDatabase CriarConexao()
        {
            var caminho = _pathProvider.ObterCaminho()
                ?? throw new InvalidOperationException("O caminho da base de dados não foi configurado.");

            return new LiteDatabase(caminho);
        }
    }
}
