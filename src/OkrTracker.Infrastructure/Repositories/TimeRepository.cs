using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;
using OkrTracker.Infrastructure.Persistence;

namespace OkrTracker.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação LiteDB do repositório de Times.
    /// </summary>
    public class TimeRepository : ITimeRepository
    {
        private readonly LiteDbConnectionFactory _connectionFactory;
        private const string CollectionName = "times";

        public TimeRepository(LiteDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<Time> ObterTodos()
        {
            using var db = _connectionFactory.CriarConexao();
            return db.GetCollection<Time>(CollectionName).FindAll().ToList();
        }

        public Time? ObterPorId(string id)
        {
            using var db = _connectionFactory.CriarConexao();
            return db.GetCollection<Time>(CollectionName).FindById(id);
        }

        public Time? ObterPorNome(string nome)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<Time>(CollectionName);
            col.EnsureIndex(t => t.Nome);
            return col.FindOne(t => t.Nome == nome);
        }

        public void Inserir(Time time)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<Time>(CollectionName);
            col.EnsureIndex(t => t.Nome, true);
            col.Insert(time);
        }

        public void Atualizar(Time time)
        {
            using var db = _connectionFactory.CriarConexao();
            db.GetCollection<Time>(CollectionName).Update(time);
        }

        public void Excluir(string id)
        {
            using var db = _connectionFactory.CriarConexao();
            db.GetCollection<Time>(CollectionName).Delete(id);
        }
    }
}
