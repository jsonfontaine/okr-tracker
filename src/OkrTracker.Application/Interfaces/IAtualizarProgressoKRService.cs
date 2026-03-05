using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para atualizar o progresso de um Key Result.
    /// Valida regras especiais: tipo Requisito só aceita 0 ou 100.
    /// </summary>
    public interface IAtualizarProgressoKRService
    {
        ResultadoOperacao<KeyResultResponse> Executar(string id, AtualizarProgressoRequest request);
    }
}
