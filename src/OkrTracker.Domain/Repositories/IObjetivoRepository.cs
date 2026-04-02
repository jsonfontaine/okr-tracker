using OkrTracker.Domain.Entities;

namespace OkrTracker.Domain.Repositories
{
    /// <summary>
    /// Porta de saída para operações de persistência de Objetivos.
    /// </summary>
    public interface IObjetivoRepository
    {
        /// <summary>
        /// Retorna objetivos filtrados por ciclo e projeto.
        /// </summary>
        IEnumerable<Objetivo> ObterPorCicloEProjeto(string cicloId, string projetoId);

        /// <summary>
        /// Retorna um objetivo pelo seu identificador.
        /// </summary>
        Objetivo? ObterPorId(string id);

        /// <summary>
        /// Verifica se existem objetivos associados a um ciclo.
        /// </summary>
        bool ExistemObjetivosParaCiclo(string cicloId);

        /// <summary>
        /// Verifica se existem objetivos associados a um projeto.
        /// </summary>
        bool ExistemObjetivosParaProjeto(string projetoId);

        /// <summary>
        /// Insere um novo objetivo.
        /// </summary>
        void Inserir(Objetivo objetivo);

        /// <summary>
        /// Atualiza um objetivo existente.
        /// </summary>
        void Atualizar(Objetivo objetivo);

        /// <summary>
        /// Remove um objetivo pelo seu identificador.
        /// </summary>
        void Excluir(string id);
    }
}
