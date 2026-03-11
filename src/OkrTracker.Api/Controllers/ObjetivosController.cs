using Microsoft.AspNetCore.Mvc;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;

namespace OkrTracker.Api.Controllers
{
    /// <summary>
    /// Controller para operações de Objetivos.
    /// </summary>
    [ApiController]
    [Route("api")]
    public class ObjetivosController : ControllerBase
    {
        private readonly ICriarObjetivoService _criarService;
        private readonly IAtualizarObjetivoService _atualizarService;
        private readonly IListarOKRsPorTimeECicloService _listarOkrsService;

        public ObjetivosController(
            ICriarObjetivoService criarService,
            IAtualizarObjetivoService atualizarService,
            IListarOKRsPorTimeECicloService listarOkrsService)
        {
            _criarService = criarService;
            _atualizarService = atualizarService;
            _listarOkrsService = listarOkrsService;
        }

        /// <summary>
        /// Lista OKRs filtrados por cicloId e projetoId (ambos obrigatórios).
        /// Retorna objetivos com KRs, comentários, fatos relevantes e riscos.
        /// </summary>
        [HttpGet("okr")]
        public IActionResult ListarOKRs([FromQuery] string cicloId, [FromQuery] string projetoId)
        {
            var resultado = _listarOkrsService.Executar(cicloId, projetoId);

            if (!resultado.Success)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        /// <summary>
        /// Cria um novo objetivo vinculado a um ciclo e time.
        /// </summary>
        [HttpPost("objetivos")]
        public IActionResult Criar([FromBody] CriarObjetivoRequest request)
        {
            var resultado = _criarService.Executar(request);

            if (!resultado.Success)
                return BadRequest(resultado);

            return Created($"/api/objetivos/{resultado.Data?.Id}", resultado);
        }

        /// <summary>
        /// Atualiza campos de um objetivo existente.
        /// </summary>
        [HttpPut("objetivos/{id}")]
        public IActionResult Atualizar(string id, [FromBody] AtualizarObjetivoRequest request)
        {
            var resultado = _atualizarService.Executar(id, request);

            if (!resultado.Success)
                return BadRequest(resultado);

            return Ok(resultado);
        }
    }
}
