namespace OkrTracker.Domain.Entities
{
    /// <summary>
    /// Representa um comentário (check-in) associado a um objetivo ou KR.
    /// Exatamente um entre ObjetivoId e KrId deve ser preenchido.
    /// </summary>
    public class Comentario
    {
        /// <summary>
        /// Identificador único do comentário.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Texto do comentário (obrigatório).
        /// </summary>
        public string Texto { get; set; } = string.Empty;

        /// <summary>
        /// Referência ao objetivo (preenchido quando o comentário é de um objetivo).
        /// </summary>
        public string? ObjetivoId { get; set; }

        /// <summary>
        /// Referência ao KR (preenchido quando o comentário é de um KR).
        /// </summary>
        public string? KrId { get; set; }

        /// <summary>
        /// Data de criação do comentário.
        /// </summary>
        public DateTime DataCriacao { get; set; }
    }
}
