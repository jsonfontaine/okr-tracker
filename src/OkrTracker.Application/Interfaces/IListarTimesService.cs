using OkrTracker.Application.DTOs;

namespace OkrTracker.Application.Interfaces
{
    /// <summary>
    /// Serviço de aplicação para listar todos os projetos.
    /// </summary>
    public interface IListarProjetosService
    {
        ResultadoOperacao<IEnumerable<ProjetoResponse>> Executar();
    }

    public interface ICriarProjetoService {
        ResultadoOperacao<ProjetoResponse> Executar(CriarProjetoRequest request);
    }
    public interface IAtualizarProjetoService {
        ResultadoOperacao<ProjetoResponse> Executar(string id, AtualizarProjetoRequest request);
    }
    public interface IExcluirProjetoService {
        ResultadoOperacao Executar(string id);
    }
}
