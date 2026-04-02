using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;
using OkrTracker.Infrastructure.Persistence;

namespace OkrTracker.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação LiteDB do repositório de Key Results.
    /// </summary>
    public class KeyResultRepository : IKeyResultRepository
    {
        private readonly LiteDbConnectionFactory _connectionFactory;
        private const string CollectionName = "krs";

        public KeyResultRepository(LiteDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<KeyResult> ObterPorObjetivoId(string objetivoId)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<KeyResult>(CollectionName);
            col.EnsureIndex(k => k.ObjetivoId);
            return col.Find(k => k.ObjetivoId == objetivoId).ToList();
        }

        public KeyResult? ObterPorId(string id)
        {
            using var db = _connectionFactory.CriarConexao();
            return db.GetCollection<KeyResult>(CollectionName).FindById(id);
        }

        public int ContarPorObjetivoId(string objetivoId)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<KeyResult>(CollectionName);
            col.EnsureIndex(k => k.ObjetivoId);
            return col.Count(k => k.ObjetivoId == objetivoId);
        }

        public void Inserir(KeyResult kr)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<KeyResult>(CollectionName);
            col.EnsureIndex(k => k.ObjetivoId);
            col.Insert(kr);
        }

        public void Atualizar(KeyResult kr)
        {
            using var db = _connectionFactory.CriarConexao();
            db.GetCollection<KeyResult>(CollectionName).Update(kr);
        }

        public void Excluir(string id)
        {
            using var db = _connectionFactory.CriarConexao();
            db.GetCollection<KeyResult>(CollectionName).Delete(id);
        }

        public void ExcluirPorObjetivoId(string objetivoId)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<KeyResult>(CollectionName);
            col.EnsureIndex(k => k.ObjetivoId);
            col.DeleteMany(k => k.ObjetivoId == objetivoId);
        }
    }
}
