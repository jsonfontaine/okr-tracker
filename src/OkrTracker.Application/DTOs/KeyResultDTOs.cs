namespace OkrTracker.Application.DTOs
{
    /// <summary>
    /// DTO para criação de um Key Result.
    /// </summary>
    public class CriarKeyResultRequest
    {
        public string ObjetivoId { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public double Progresso { get; set; }
        public string Farol { get; set; } = "Verde";
        public bool Intruder { get; set; }
        public bool DescobertaTardia { get; set; }
    }

    /// <summary>
    /// DTO para atualização de um Key Result.
    /// </summary>
    public class AtualizarKeyResultRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Farol { get; set; } = "Verde";
        public string Status { get; set; } = "NaoIniciado";
        public bool Intruder { get; set; }
        public bool DescobertaTardia { get; set; }
    }

    /// <summary>
    /// DTO para atualização do progresso de um Key Result.
    /// </summary>
    public class AtualizarProgressoRequest
    {
        public double Progresso { get; set; }
    }

    /// <summary>
    /// DTO de resposta de um Key Result.
    /// </summary>
    public class KeyResultResponse
    {
        public string Id { get; set; } = string.Empty;
        public string ObjetivoId { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public double Progresso { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Farol { get; set; } = string.Empty;
        public bool Intruder { get; set; }
        public bool DescobertaTardia { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime UltimaAtualizacao { get; set; }
        public List<ComentarioResponse> Comentarios { get; set; } = new();
        public List<FatoRelevanteResponse> FatosRelevantes { get; set; } = new();
        public List<RiscoResponse> Riscos { get; set; } = new();
    }
}
