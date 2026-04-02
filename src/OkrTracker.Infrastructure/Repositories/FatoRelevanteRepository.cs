using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;
using OkrTracker.Infrastructure.Persistence;

namespace OkrTracker.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação LiteDB do repositório de Fatos Relevantes.
    /// </summary>
    public class FatoRelevanteRepository : IFatoRelevanteRepository
    {
        private readonly LiteDbConnectionFactory _connectionFactory;
        private const string CollectionName = "fatosRelevantes";

        public FatoRelevanteRepository(LiteDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<FatoRelevante> ObterPorObjetivoId(string objetivoId)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<FatoRelevante>(CollectionName);
            col.EnsureIndex(f => f.ObjetivoId);
            return col.Find(f => f.ObjetivoId == objetivoId)
                .OrderByDescending(f => f.DataCriacao)
                .ToList();
        }

        public IEnumerable<FatoRelevante> ObterPorKrId(string krId)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<FatoRelevante>(CollectionName);
            col.EnsureIndex(f => f.KrId);
            return col.Find(f => f.KrId == krId)
                .OrderByDescending(f => f.DataCriacao)
                .ToList();
        }

        public void Inserir(FatoRelevante fatoRelevante)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<FatoRelevante>(CollectionName);
            col.EnsureIndex(f => f.ObjetivoId);
            col.EnsureIndex(f => f.KrId);
            col.Insert(fatoRelevante);
        }

        public void ExcluirPorObjetivoId(string objetivoId)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<FatoRelevante>(CollectionName);
            col.EnsureIndex(f => f.ObjetivoId);
            col.DeleteMany(f => f.ObjetivoId == objetivoId);
        }

        public void ExcluirPorKrId(string krId)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<FatoRelevante>(CollectionName);
            col.EnsureIndex(f => f.KrId);
            col.DeleteMany(f => f.KrId == krId);
        }
    }
}
