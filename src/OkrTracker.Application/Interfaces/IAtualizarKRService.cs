using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para atualizar campos de um Key Result.
    /// </summary>
    public interface IAtualizarKRService
    {
        ResultadoOperacao<KeyResultResponse> Executar(string id, AtualizarKeyResultRequest request);
    }
}
