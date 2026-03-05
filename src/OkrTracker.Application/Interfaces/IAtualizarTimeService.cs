using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para atualizar um time existente.
    /// </summary>
    public interface IAtualizarTimeService
    {
        ResultadoOperacao<TimeResponse> Executar(string id, AtualizarTimeRequest request);
    }
}
