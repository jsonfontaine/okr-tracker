namespace OkrTracker.Application.DTOs
{
    /// <summary>
    /// DTO para criação de um time.
    /// </summary>
    public class CriarTimeRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }

    /// <summary>
    /// DTO para atualização de um time.
    /// </summary>
    public class AtualizarTimeRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }

    /// <summary>
    /// DTO de resposta de um time.
    /// </summary>
    public class TimeResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime UltimaAtualizacao { get; set; }
    }
}
