namespace OkrTracker.Application.DTOs
{
    /// <summary>
    /// DTO para criação de um risco.
    /// </summary>
    public class CriarRiscoRequest
    {
        public string? ObjetivoId { get; set; }
        public string? KrId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string? Impacto { get; set; }
    }

    /// <summary>
    /// DTO de resposta de um risco.
    /// </summary>
    public class RiscoResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string? Impacto { get; set; }
        public string? ObjetivoId { get; set; }
        public string? KrId { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
