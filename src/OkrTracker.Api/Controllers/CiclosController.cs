using Microsoft.AspNetCore.Mvc;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;

namespace OkrTracker.Api.Controllers
{
    /// <summary>
    /// Controller para operações de Ciclos.
    /// </summary>
    [ApiController]
    [Route("api/ciclos")]
    public class CiclosController : ControllerBase
    {
        private readonly ICriarCicloService _criarService;
        private readonly IListarCiclosService _listarService;
        private readonly IAtualizarCicloService _atualizarService;
        private readonly IExcluirCicloService _excluirService;

        public CiclosController(
            ICriarCicloService criarService,
            IListarCiclosService listarService,
            IAtualizarCicloService atualizarService,
            IExcluirCicloService excluirService)
        {
            _criarService = criarService;
            _listarService = listarService;
            _atualizarService = atualizarService;
            _excluirService = excluirService;
        }

        /// <summary>
        /// Retorna lista de todos os ciclos.
        /// </summary>
        [HttpGet]
        public IActionResult Listar()
        {
            var resultado = _listarService.Executar();
            return Ok(resultado);
        }

        /// <summary>
        /// Cria um novo ciclo.
        /// </summary>
        [HttpPost]
        public IActionResult Criar([FromBody] CriarCicloRequest request)
        {
            var resultado = _criarService.Executar(request);

            if (!resultado.Success)
                return BadRequest(resultado);

            return Created($"/api/ciclos/{resultado.Data?.Id}", resultado);
        }

        /// <summary>
        /// Atualiza o nome de um ciclo existente.
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Atualizar(string id, [FromBody] AtualizarCicloRequest request)
        {
            var resultado = _atualizarService.Executar(id, request);

            if (!resultado.Success)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        /// <summary>
        /// Exclui um ciclo (somente se não houver objetivos associados).
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
