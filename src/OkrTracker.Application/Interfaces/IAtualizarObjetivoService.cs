using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para atualizar um objetivo existente.
    /// </summary>
    public interface IAtualizarObjetivoService
    {
        ResultadoOperacao<ObjetivoResponse> Executar(string id, AtualizarObjetivoRequest request);
    }
}
