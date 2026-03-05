using OkrTracker.Domain.Entities;

namespace OkrTracker.Domain.Repositories
{
    /// <summary>
    /// Porta de saída para operações de persistência de Times.
    /// </summary>
    public interface ITimeRepository
    {
        /// <summary>
        /// Retorna todos os times cadastrados.
        /// </summary>
        IEnumerable<Time> ObterTodos();

        /// <summary>
        /// Retorna um time pelo seu identificador.
        /// </summary>
        Time? ObterPorId(string id);

        /// <summary>
        /// Retorna um time pelo nome.
        /// </summary>
        Time? ObterPorNome(string nome);

        /// <summary>
        /// Insere um novo time.
        /// </summary>
        void Inserir(Time time);

        /// <summary>
        /// Atualiza um time existente.
        /// </summary>
        void Atualizar(Time time);

        /// <summary>
        /// Remove um time pelo seu identificador.
        /// </summary>
        void Excluir(string id);
    }
}
