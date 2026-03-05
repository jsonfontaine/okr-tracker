namespace OkrTracker.Domain.Entities
{
    /// <summary>
    /// Representa um time ou frente de trabalho.
    /// </summary>
    public class Time
    {
        /// <summary>
        /// Identificador único do time.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Nome do time. Deve ser único.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Descrição opcional do time.
        /// </summary>
        public string? Descricao { get; set; }

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
