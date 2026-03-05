using OkrTracker.Domain.Enums;

namespace OkrTracker.Domain.Entities
{
    /// <summary>
    /// Representa um Key Result vinculado a um objetivo.
    /// Pode ser do tipo Quantitativo, Qualitativo ou Requisito.
    /// </summary>
    public class KeyResult
    {
        /// <summary>
        /// Identificador único do KR.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Referência ao objetivo pai.
        /// </summary>
        public string ObjetivoId { get; set; } = string.Empty;

        /// <summary>
        /// Título do KR (obrigatório).
        /// </summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Descrição detalhada do KR (obrigatória).
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Tipo do KR: Quantitativo, Qualitativo ou Requisito.
        /// </summary>
        public TipoKR Tipo { get; set; }

        /// <summary>
        /// Progresso atual do KR (0 a 100).
        /// Para tipo Requisito, só aceita 0 ou 100.
        /// </summary>
        public double Progresso { get; set; }

        /// <summary>
        /// Status calculado do KR com base no progresso.
        /// </summary>
        public Status Status { get; set; } = Status.NaoIniciado;

        /// <summary>
        /// Farol visual (Verde, Amarelo, Vermelho) para indicar situação do KR.
        /// </summary>
        public Farol Farol { get; set; } = Farol.Verde;

        /// <summary>
        /// Indica se o KR é um "intruder" (demanda não planejada).
        /// </summary>
        public bool Intruder { get; set; }

        /// <summary>
        /// Indica se o KR é uma descoberta tardia.
        /// </summary>
        public bool DescobertaTardia { get; set; }

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
