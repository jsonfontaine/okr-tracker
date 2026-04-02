using OkrTracker.Domain.Entities;

namespace OkrTracker.Domain.Repositories
{
    /// <summary>
    /// Porta de saída para operações de persistência de Key Results.
    /// </summary>
    public interface IKeyResultRepository
    {
        /// <summary>
        /// Retorna todos os KRs de um objetivo.
        /// </summary>
        IEnumerable<KeyResult> ObterPorObjetivoId(string objetivoId);

        /// <summary>
        /// Retorna um KR pelo seu identificador.
        /// </summary>
        KeyResult? ObterPorId(string id);

        /// <summary>
        /// Conta quantos KRs pertencem a um objetivo.
        /// </summary>
        int ContarPorObjetivoId(string objetivoId);

        /// <summary>
        /// Insere um novo KR.
        /// </summary>
        void Inserir(KeyResult kr);

        /// <summary>
        /// Atualiza um KR existente.
        /// </summary>
        void Atualizar(KeyResult kr);

        /// <summary>
        /// Remove um KR pelo seu identificador.
        /// </summary>
        void Excluir(string id);

        /// <summary>
        /// Remove todos os KRs de um objetivo.
        /// </summary>
        void ExcluirPorObjetivoId(string objetivoId);
    }
}
