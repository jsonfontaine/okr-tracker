namespace OkrTracker.Domain.Entities
{
    /// <summary>
    /// Representa um fato relevante associado a um objetivo ou KR.
    /// Exatamente um entre ObjetivoId e KrId deve ser preenchido.
    /// </summary>
    public class FatoRelevante
    {
        /// <summary>
        /// Identificador único do fato relevante.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Texto descritivo do fato relevante (obrigatório).
        /// </summary>
        public string Texto { get; set; } = string.Empty;

        /// <summary>
        /// Referência ao objetivo (preenchido quando o fato é de um objetivo).
        /// </summary>
        public string? ObjetivoId { get; set; }

        /// <summary>
        /// Referência ao KR (preenchido quando o fato é de um KR).
        /// </summary>
        public string? KrId { get; set; }

        /// <summary>
        /// Data de criação do fato relevante.
        /// </summary>
        public DateTime DataCriacao { get; set; }
    }
}
