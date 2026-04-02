using OkrTracker.Domain.Entities;

namespace OkrTracker.Domain.Repositories
{
    /// <summary>
    /// Porta de saída para operações de persistência de Fatos Relevantes.
    /// </summary>
    public interface IFatoRelevanteRepository
    {
        /// <summary>
        /// Retorna fatos relevantes de um objetivo.
        /// </summary>
        IEnumerable<FatoRelevante> ObterPorObjetivoId(string objetivoId);

        /// <summary>
        /// Retorna fatos relevantes de um KR.
        /// </summary>
        IEnumerable<FatoRelevante> ObterPorKrId(string krId);

        /// <summary>
        /// Insere um novo fato relevante.
        /// </summary>
        void Inserir(FatoRelevante fatoRelevante);

        /// <summary>
        /// Remove todos os fatos relevantes de um objetivo.
        /// </summary>
        void ExcluirPorObjetivoId(string objetivoId);

        /// <summary>
        /// Remove todos os fatos relevantes de um KR.
        /// </summary>
        void ExcluirPorKrId(string krId);
    }
}
