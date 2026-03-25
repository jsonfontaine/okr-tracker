namespace OkrTracker.Application.DTOs
{
    /// <summary>
    /// DTO para criação de um ciclo.
    /// </summary>
    public class CriarCicloRequest
    {
        public string Nome { get; set; } = string.Empty;
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
    }

    /// <summary>
    /// DTO para atualização de um ciclo.
    /// </summary>
    public class AtualizarCicloRequest
    {
        public string Nome { get; set; } = string.Empty;
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
    }

    /// <summary>
    /// DTO de resposta de um ciclo.
    /// </summary>
    public class CicloResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime UltimaAtualizacao { get; set; }
    }
}
