using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;
using OkrTracker.Infrastructure.Persistence;

namespace OkrTracker.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação LiteDB do repositório de Projetos.
    /// </summary>
    public class ProjetoRepository : IProjetoRepository
    {
        private readonly LiteDbConnectionFactory _connectionFactory;
        private const string CollectionName = "projetos";

        public ProjetoRepository(LiteDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<Projeto> ObterTodos()
        {
            using var db = _connectionFactory.CriarConexao();
            return db.GetCollection<Projeto>(CollectionName).FindAll().ToList();
        }

        public Projeto? ObterPorId(string id)
        {
            using var db = _connectionFactory.CriarConexao();
            return db.GetCollection<Projeto>(CollectionName).FindById(id);
        }

        public Projeto? ObterPorNome(string nome)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<Projeto>(CollectionName);
            col.EnsureIndex(t => t.Nome);
            return col.FindOne(t => t.Nome == nome);
        }

        public void Inserir(Projeto projeto)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<Projeto>(CollectionName);
            col.EnsureIndex(t => t.Nome, true);
            col.Insert(projeto);
        }

        public void Atualizar(Projeto projeto)
        {
            using var db = _connectionFactory.CriarConexao();
            db.GetCollection<Projeto>(CollectionName).Update(projeto);
        }

        public void Excluir(string id)
        {
            using var db = _connectionFactory.CriarConexao();
            db.GetCollection<Projeto>(CollectionName).Delete(id);
        }
    }
}
