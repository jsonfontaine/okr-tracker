using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;
using OkrTracker.Infrastructure.Persistence;

namespace OkrTracker.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação LiteDB do repositório de Objetivos.
    /// </summary>
    public class ObjetivoRepository : IObjetivoRepository
    {
        private readonly LiteDbConnectionFactory _connectionFactory;
        private const string CollectionName = "objetivos";

        public ObjetivoRepository(LiteDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<Objetivo> ObterPorCicloEProjeto(string cicloId, string projetoId)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<Objetivo>(CollectionName);
            col.EnsureIndex(o => o.CicloId);
            col.EnsureIndex(o => o.ProjetoId);
            return col.Find(o => o.CicloId == cicloId && o.ProjetoId == projetoId).ToList();
        }

        public Objetivo? ObterPorId(string id)
        {
            using var db = _connectionFactory.CriarConexao();
            return db.GetCollection<Objetivo>(CollectionName).FindById(id);
        }

        public bool ExistemObjetivosParaCiclo(string cicloId)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<Objetivo>(CollectionName);
            col.EnsureIndex(o => o.CicloId);
            return col.Exists(o => o.CicloId == cicloId);
        }

        public bool ExistemObjetivosParaProjeto(string projetoId)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<Objetivo>(CollectionName);
            col.EnsureIndex(o => o.ProjetoId);
            return col.Exists(o => o.ProjetoId == projetoId);
        }

        public void Inserir(Objetivo objetivo)
        {
            using var db = _connectionFactory.CriarConexao();
            var col = db.GetCollection<Objetivo>(CollectionName);
            col.EnsureIndex(o => o.CicloId);
            col.EnsureIndex(o => o.ProjetoId);
            col.Insert(objetivo);
        }

        public void Atualizar(Objetivo objetivo)
        {
            using var db = _connectionFactory.CriarConexao();
            db.GetCollection<Objetivo>(CollectionName).Update(objetivo);
        }
    }
}
