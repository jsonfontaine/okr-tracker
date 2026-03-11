using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para listar OKRs filtrados por ciclo e projeto.
    /// Retorna objetivos com seus KRs, comentários, fatos relevantes e riscos.
    /// </summary>
    public interface IListarOKRsPorTimeECicloService
    {
        ResultadoOperacao<IEnumerable<ObjetivoResponse>> Executar(string cicloId, string projetoId);
    }
}
