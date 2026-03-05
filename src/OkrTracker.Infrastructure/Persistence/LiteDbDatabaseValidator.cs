using LiteDB;
using OkrTracker.Application.Ports;

namespace OkrTracker.Infrastructure.Persistence
{
    /// <summary>
    /// Validador que tenta abrir o arquivo LiteDB no caminho informado.
    /// Retorna true se o arquivo pode ser aberto com sucesso.
    /// </summary>
    public class LiteDbDatabaseValidator : IDatabaseValidator
    {
        public bool Validar(string databasePath)
        {
            try
            {
                using var db = new LiteDatabase(databasePath);
                // Tenta acessar uma coleção para validar que o banco funciona
                db.GetCollectionNames();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
