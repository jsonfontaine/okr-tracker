namespace OkrTracker.Domain.Entities
{
    /// <summary>
    /// Representa um ciclo de OKR (ex.: 2026-Q1, 2026-S1).
    /// </summary>
    public class Ciclo
    {
        /// <summary>
        /// Identificador único do ciclo.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Nome do ciclo (ex.: 2026-Q1). Deve ser único.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Data de início do ciclo (opcional para compatibilidade com ciclos legados).
        /// </summary>
        public DateTime? DataInicio { get; set; }

        /// <summary>
        /// Data de término do ciclo (opcional para compatibilidade com ciclos legados).
        /// </summary>
        public DateTime? DataFim { get; set; }

        /// <summary>
        /// Data de criação do registro.
        /// </summary>
        public DateTime DataCriacao { get; set; }

        /// <summary>
        /// Data da última atualização do registro.
        /// </summary>
        public DateTime UltimaAtualizacao { get; set; }
    }
}
