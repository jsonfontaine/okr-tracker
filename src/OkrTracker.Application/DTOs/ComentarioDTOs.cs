namespace OkrTracker.Application.DTOs
{
    /// <summary>
    /// DTO para criação de um comentário.
    /// </summary>
    public class CriarComentarioRequest
    {
        public string? ObjetivoId { get; set; }
        public string? KrId { get; set; }
        public string Texto { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO de resposta de um comentário.
    /// </summary>
    public class ComentarioResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;
        public string? ObjetivoId { get; set; }
        public string? KrId { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
