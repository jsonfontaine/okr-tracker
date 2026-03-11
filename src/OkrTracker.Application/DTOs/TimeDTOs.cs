namespace OkrTracker.Application.DTOs
{
    /// <summary>
    /// DTO para criação de um projeto.
    /// </summary>
    public class CriarProjetoRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }

    /// <summary>
    /// DTO para atualização de um projeto.
    /// </summary>
    public class AtualizarProjetoRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }

    /// <summary>
    /// DTO de resposta de um projeto.
    /// </summary>
    public class ProjetoResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime UltimaAtualizacao { get; set; }
    }
}
