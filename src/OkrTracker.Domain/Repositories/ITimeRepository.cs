using OkrTracker.Domain.Entities;

namespace OkrTracker.Domain.Repositories
{
    /// <summary>
    /// Porta de saída para operações de persistência de Projetos.
    /// </summary>
    public interface IProjetoRepository
    {
        /// <summary>
        /// Retorna todos os projetos cadastrados.
        /// </summary>
        IEnumerable<Projeto> ObterTodos();

        /// <summary>
        /// Retorna um projeto pelo seu identificador.
        /// </summary>
        Projeto? ObterPorId(string id);

        /// <summary>
        /// Retorna um projeto pelo nome.
        /// </summary>
        Projeto? ObterPorNome(string nome);

        /// <summary>
        /// Insere um novo projeto.
        /// </summary>
        void Inserir(Projeto projeto);

        /// <summary>
        /// Atualiza um projeto existente.
        /// </summary>
        void Atualizar(Projeto projeto);

        /// <summary>
        /// Remove um projeto pelo seu identificador.
        /// </summary>
        void Excluir(string id);
    }
}
