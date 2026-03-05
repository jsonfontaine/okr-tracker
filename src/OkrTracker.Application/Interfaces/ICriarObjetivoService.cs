using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para criar um novo objetivo vinculado a um ciclo e time.
    /// </summary>
    public interface ICriarObjetivoService
    {
        ResultadoOperacao<ObjetivoResponse> Executar(CriarObjetivoRequest request);
    }
}
