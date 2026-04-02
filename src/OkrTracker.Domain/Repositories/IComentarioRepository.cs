using OkrTracker.Domain.Entities;

namespace OkrTracker.Domain.Repositories
{
    /// <summary>
    /// Porta de saída para operações de persistência de Comentários (check-ins).
    /// </summary>
    public interface IComentarioRepository
    {
        /// <summary>
        /// Retorna comentários de um objetivo, ordenados do mais recente para o mais antigo.
        /// </summary>
        IEnumerable<Comentario> ObterPorObjetivoId(string objetivoId);

        /// <summary>
        /// Retorna comentários de um KR, ordenados do mais recente para o mais antigo.
        /// </summary>
        IEnumerable<Comentario> ObterPorKrId(string krId);

        /// <summary>
        /// Insere um novo comentário.
        /// </summary>
        void Inserir(Comentario comentario);

        /// <summary>
        /// Remove todos os comentários de um objetivo.
        /// </summary>
        void ExcluirPorObjetivoId(string objetivoId);

        /// <summary>
        /// Remove todos os comentários de um KR.
        /// </summary>
        void ExcluirPorKrId(string krId);
    }
}
