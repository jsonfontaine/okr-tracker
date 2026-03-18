using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para atualizar o progresso de um Key Result.
    /// Valida a faixa de progresso entre 0 e 100.
    /// </summary>
    public interface IAtualizarProgressoKRService
    {
        ResultadoOperacao<KeyResultResponse> Executar(string id, AtualizarProgressoRequest request);
    }
}
