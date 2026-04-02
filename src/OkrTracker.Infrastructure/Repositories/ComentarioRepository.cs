using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;
using OkrTracker.Infrastructure.Persistence;

namespace OkrTracker.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação LiteDB do repositório de Comentários.
    /// </summary>
    public class ComentarioRepository : IComentarioRepository
    {
        private readonly LiteDbConnectionFactory _connectionFactory;
        private const string CollectionName = "comentarios";

        public ComentarioRepository(LiteDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<Comentario> ObterPorObjetivoId(string objetivoId)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<Comentario>(CollectionName);
            col.EnsureIndex(c => c.ObjetivoId);
            return col.Find(c => c.ObjetivoId == objetivoId)
                .OrderByDescending(c => c.DataCriacao)
                .ToList();
        }

        public IEnumerable<Comentario> ObterPorKrId(string krId)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<Comentario>(CollectionName);
            col.EnsureIndex(c => c.KrId);
            return col.Find(c => c.KrId == krId)
                .OrderByDescending(c => c.DataCriacao)
                .ToList();
        }

        public void Inserir(Comentario comentario)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<Comentario>(CollectionName);
            col.EnsureIndex(c => c.ObjetivoId);
            col.EnsureIndex(c => c.KrId);
            col.Insert(comentario);
        }

        public void ExcluirPorObjetivoId(string objetivoId)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<Comentario>(CollectionName);
            col.EnsureIndex(c => c.ObjetivoId);
            col.DeleteMany(c => c.ObjetivoId == objetivoId);
        }

        public void ExcluirPorKrId(string krId)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<Comentario>(CollectionName);
            col.EnsureIndex(c => c.KrId);
            col.DeleteMany(c => c.KrId == krId);
        }
    }
}
