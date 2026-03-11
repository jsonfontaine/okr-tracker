using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para atualizar um projeto existente.
    /// </summary>
    public interface IAtualizarTimeService
    {
        ResultadoOperacao<ProjetoResponse> Executar(string id, AtualizarProjetoRequest request);
    }
}
