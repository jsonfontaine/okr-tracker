namespace OkrTracker.Application.DTOs
{
    /// <summary>
    /// DTO para criação de um objetivo.
    /// </summary>
    public class CriarObjetivoRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string CicloId { get; set; } = string.Empty;
        public string TimeId { get; set; } = string.Empty;
        public string Prioridade { get; set; } = "Media";
        public string Farol { get; set; } = "Verde";
        public bool Intruder { get; set; }
        public bool DescobertaTardia { get; set; }
        public string Valor { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para atualização de um objetivo.
    /// </summary>
    public class AtualizarObjetivoRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string CicloId { get; set; } = string.Empty;
        public string TimeId { get; set; } = string.Empty;
        public string Prioridade { get; set; } = "Media";
        public string Farol { get; set; } = "Verde";
        public bool Intruder { get; set; }
        public bool DescobertaTardia { get; set; }
        public string Valor { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO de resposta de um objetivo com seus KRs e eventos.
    /// </summary>
    public class ObjetivoResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string CicloId { get; set; } = string.Empty;
        public string TimeId { get; set; } = string.Empty;
        public string Prioridade { get; set; } = string.Empty;
        public double Progresso { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Farol { get; set; } = string.Empty;
        public bool Intruder { get; set; }
        public bool DescobertaTardia { get; set; }
        public string Valor { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public DateTime UltimaAtualizacao { get; set; }
        public List<KeyResultResponse> KeyResults { get; set; } = new();
        public List<ComentarioResponse> Comentarios { get; set; } = new();
        public List<FatoRelevanteResponse> FatosRelevantes { get; set; } = new();
        public List<RiscoResponse> Riscos { get; set; } = new();
    }
}
