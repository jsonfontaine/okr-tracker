using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;
using OkrTracker.Infrastructure.Persistence;

namespace OkrTracker.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação LiteDB do repositório de Ciclos.
    /// </summary>
    public class CicloRepository : ICicloRepository
    {
        private readonly LiteDbConnectionFactory _connectionFactory;
        private const string CollectionName = "ciclos";

        public CicloRepository(LiteDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<Ciclo> ObterTodos()
        {
            using var db = _connectionFactory.CriarConexao();
            return db.GetCollection<Ciclo>(CollectionName).FindAll().ToList();
        }

        public Ciclo? ObterPorId(string id)
        {
            using var db = _connectionFactory.CriarConexao();
            return db.GetCollection<Ciclo>(CollectionName).FindById(id);
        }

        public Ciclo? ObterPorNome(string nome)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<Ciclo>(CollectionName);
            col.EnsureIndex(c => c.Nome);
            return col.FindOne(c => c.Nome == nome);
        }

        public void Inserir(Ciclo ciclo)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<Ciclo>(CollectionName);
            col.EnsureIndex(c => c.Nome, true);
            col.Insert(ciclo);
        }

        public void Atualizar(Ciclo ciclo)
        {
            using var db = _connectionFactory.CriarConexao();
            db.GetCollection<Ciclo>(CollectionName).Update(ciclo);
        }

        public void Excluir(string id)
        {
            using var db = _connectionFactory.CriarConexao();
            db.GetCollection<Ciclo>(CollectionName).Delete(id);
        }
    }
}
