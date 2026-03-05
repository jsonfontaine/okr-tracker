namespace OkrTracker.Domain.Entities
{
    /// <summary>
    /// Representa um risco ou ponto de atenção associado a um objetivo ou KR.
    /// Exatamente um entre ObjetivoId e KrId deve ser preenchido.
    /// </summary>
    public class Risco
    {
        /// <summary>
        /// Identificador único do risco.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do risco (obrigatória).
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Impacto do risco (texto livre, opcional).
        /// </summary>
        public string? Impacto { get; set; }

        /// <summary>
        /// Referência ao objetivo (preenchido quando o risco é de um objetivo).
        /// </summary>
        public string? ObjetivoId { get; set; }

        /// <summary>
        /// Referência ao KR (preenchido quando o risco é de um KR).
        /// </summary>
        public string? KrId { get; set; }

        /// <summary>
        /// Data de criação do risco.
        /// </summary>
        public DateTime DataCriacao { get; set; }
    }
}
