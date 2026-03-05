using OkrTracker.Domain.Entities;

namespace OkrTracker.Domain.Repositories
{
    /// <summary>
    /// Porta de saída para operações de persistência de Ciclos.
    /// </summary>
    public interface ICicloRepository
    {
        /// <summary>
        /// Retorna todos os ciclos cadastrados.
        /// </summary>
        IEnumerable<Ciclo> ObterTodos();

        /// <summary>
        /// Retorna um ciclo pelo seu identificador.
        /// </summary>
        Ciclo? ObterPorId(string id);

        /// <summary>
        /// Retorna um ciclo pelo nome.
        /// </summary>
        Ciclo? ObterPorNome(string nome);

        /// <summary>
        /// Insere um novo ciclo.
        /// </summary>
        void Inserir(Ciclo ciclo);

        /// <summary>
        /// Atualiza um ciclo existente.
        /// </summary>
        void Atualizar(Ciclo ciclo);

        /// <summary>
        /// Remove um ciclo pelo seu identificador.
        /// </summary>
        void Excluir(string id);
    }
}
