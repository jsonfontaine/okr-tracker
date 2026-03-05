using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para atualizar um ciclo existente.
    /// </summary>
    public interface IAtualizarCicloService
    {
        ResultadoOperacao<CicloResponse> Executar(string id, AtualizarCicloRequest request);
    }
}
