using Microsoft.AspNetCore.Mvc;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;

namespace OkrTracker.Api.Controllers
{
    /// <summary>
    /// Controller para operações de Projetos.
    /// </summary>
    [ApiController]
    [Route("api/projetos")]
    public class ProjetosController : ControllerBase
    {
        private readonly ICriarProjetoService _criarService;
        private readonly IListarProjetosService _listarService;
        private readonly IAtualizarProjetoService _atualizarService;
        private readonly IExcluirProjetoService _excluirService;

        public ProjetosController(
            ICriarProjetoService criarService,
            IListarProjetosService listarService,
            IAtualizarProjetoService atualizarService,
            IExcluirProjetoService excluirService)
        {
            _criarService = criarService;
            _listarService = listarService;
            _atualizarService = atualizarService;
            _excluirService = excluirService;
        }

        /// <summary>
        /// Retorna lista de todos os projetos.
        /// </summary>
        [HttpGet]
        public IActionResult Listar()
        {
            var resultado = _listarService.Executar();
            return Ok(resultado);
        }

        /// <summary>
        /// Cria um novo projeto.
        /// </summary>
        [HttpPost]
        public IActionResult Criar([FromBody] CriarProjetoRequest request)
        {
            var resultado = _criarService.Executar(request);

            if (!resultado.Success)
                return BadRequest(resultado);

            return Created($"/api/projetos/{resultado.Data?.Id}", resultado);
        }

        /// <summary>
        /// Atualiza nome/descrição de um projeto existente.
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Atualizar(string id, [FromBody] AtualizarProjetoRequest request)
        {
            var resultado = _atualizarService.Executar(id, request);

            if (!resultado.Success)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        /// <summary>
        /// Exclui um projeto (somente se não houver objetivos associados).
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Excluir(string id)
        {
            var resultado = _excluirService.Executar(id);

            if (!resultado.Success)
                return BadRequest(resultado);

            return Ok(resultado);
        }
    }
}
