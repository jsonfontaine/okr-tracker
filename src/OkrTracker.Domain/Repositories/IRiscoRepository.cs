using OkrTracker.Domain.Entities;

namespace OkrTracker.Domain.Repositories
{
    /// <summary>
    /// Porta de saída para operações de persistência de Riscos.
    /// </summary>
    public interface IRiscoRepository
    {
        /// <summary>
        /// Retorna riscos de um objetivo.
        /// </summary>
        IEnumerable<Risco> ObterPorObjetivoId(string objetivoId);

        /// <summary>
        /// Retorna riscos de um KR.
        /// </summary>
        IEnumerable<Risco> ObterPorKrId(string krId);

        /// <summary>
        /// Insere um novo risco.
        /// </summary>
        void Inserir(Risco risco);

        /// <summary>
        /// Remove todos os riscos de um objetivo.
        /// </summary>
        void ExcluirPorObjetivoId(string objetivoId);

        /// <summary>
        /// Remove todos os riscos de um KR.
        /// </summary>
        void ExcluirPorKrId(string krId);
    }
}
