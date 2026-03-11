namespace OkrTracker.Domain.Entities
{
    /// <summary>
    /// Representa um projeto ou frente de trabalho.
    /// </summary>
    public class Projeto
    {
        /// <summary>
        /// Identificador único do projeto.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Nome do projeto. Deve ser único.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Descrição opcional do projeto.
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
