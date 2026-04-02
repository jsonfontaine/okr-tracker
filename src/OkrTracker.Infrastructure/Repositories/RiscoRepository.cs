using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;
using OkrTracker.Infrastructure.Persistence;

namespace OkrTracker.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação LiteDB do repositório de Riscos.
    /// </summary>
    public class RiscoRepository : IRiscoRepository
    {
        private readonly LiteDbConnectionFactory _connectionFactory;
        private const string CollectionName = "riscos";

        public RiscoRepository(LiteDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<Risco> ObterPorObjetivoId(string objetivoId)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<Risco>(CollectionName);
            col.EnsureIndex(r => r.ObjetivoId);
            return col.Find(r => r.ObjetivoId == objetivoId)
                .OrderByDescending(r => r.DataCriacao)
                .ToList();
        }

        public IEnumerable<Risco> ObterPorKrId(string krId)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<Risco>(CollectionName);
            col.EnsureIndex(r => r.KrId);
            return col.Find(r => r.KrId == krId)
                .OrderByDescending(r => r.DataCriacao)
                .ToList();
        }

        public void Inserir(Risco risco)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<Risco>(CollectionName);
            col.EnsureIndex(r => r.ObjetivoId);
            col.EnsureIndex(r => r.KrId);
            col.Insert(risco);
        }

        public void ExcluirPorObjetivoId(string objetivoId)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<Risco>(CollectionName);
            col.EnsureIndex(r => r.ObjetivoId);
            col.DeleteMany(r => r.ObjetivoId == objetivoId);
        }

        public void ExcluirPorKrId(string krId)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<Risco>(CollectionName);
            col.EnsureIndex(r => r.KrId);
            col.DeleteMany(r => r.KrId == krId);
        }
    }
}
