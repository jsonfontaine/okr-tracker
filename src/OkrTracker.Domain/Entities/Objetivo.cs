using OkrTracker.Domain.Enums;

namespace OkrTracker.Domain.Entities
{
    /// <summary>
    /// Representa um objetivo vinculado a um ciclo e um time.
    /// O progresso é calculado automaticamente com base na média dos KRs.
    /// </summary>
    public class Objetivo
    {
        /// <summary>
        /// Identificador único do objetivo.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Título do objetivo (obrigatório).
        /// </summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Descrição detalhada do objetivo (obrigatória).
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Referência ao ciclo ao qual o objetivo pertence.
        /// </summary>
        public string CicloId { get; set; } = string.Empty;

        /// <summary>
        /// Referência ao time responsável pelo objetivo.
        /// </summary>
        public string TimeId { get; set; } = string.Empty;

        /// <summary>
        /// Prioridade do objetivo (Alta, Média, Baixa).
        /// </summary>
        public Prioridade Prioridade { get; set; } = Prioridade.Media;

        /// <summary>
        /// Progresso calculado do objetivo (0 a 100), baseado na média dos KRs.
        /// </summary>
        public double Progresso { get; set; }

        /// <summary>
        /// Status calculado do objetivo com base no progresso.
        /// </summary>
        public Status Status { get; set; } = Status.NaoIniciado;

        /// <summary>
        /// Farol visual (Verde, Amarelo, Vermelho) para indicar situação do objetivo.
        /// </summary>
        public Farol Farol { get; set; } = Farol.Verde;

        /// <summary>
        /// Indica se o objetivo é um "intruder" (demanda não planejada que entrou no ciclo).
        /// </summary>
        public bool Intruder { get; set; }

        /// <summary>
        /// Indica se o objetivo é uma descoberta tardia no ciclo.
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
