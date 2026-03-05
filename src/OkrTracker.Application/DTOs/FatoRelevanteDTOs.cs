namespace OkrTracker.Application.DTOs
{
    /// <summary>
    /// DTO para criação de um fato relevante.
    /// </summary>
    public class CriarFatoRelevanteRequest
    {
        public string? ObjetivoId { get; set; }
        public string? KrId { get; set; }
        public string Texto { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO de resposta de um fato relevante.
    /// </summary>
    public class FatoRelevanteResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;
        public string? ObjetivoId { get; set; }
        public string? KrId { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
