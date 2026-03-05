using Microsoft.AspNetCore.Mvc;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;

namespace OkrTracker.Api.Controllers
{
    /// <summary>
    /// Controller para operações de Times.
    /// </summary>
    [ApiController]
    [Route("api/times")]
    public class TimesController : ControllerBase
    {
        private readonly ICriarTimeService _criarService;
        private readonly IListarTimesService _listarService;
        private readonly IAtualizarTimeService _atualizarService;
        private readonly IExcluirTimeService _excluirService;

        public TimesController(
            ICriarTimeService criarService,
            IListarTimesService listarService,
            IAtualizarTimeService atualizarService,
            IExcluirTimeService excluirService)
        {
            _criarService = criarService;
            _listarService = listarService;
            _atualizarService = atualizarService;
            _excluirService = excluirService;
        }

        /// <summary>
        /// Retorna lista de todos os times.
        /// </summary>
        [HttpGet]
        public IActionResult Listar()
        {
            var resultado = _listarService.Executar();
            return Ok(resultado);
        }

        /// <summary>
        /// Cria um novo time.
        /// </summary>
        [HttpPost]
        public IActionResult Criar([FromBody] CriarTimeRequest request)
        {
            var resultado = _criarService.Executar(request);

            if (!resultado.Success)
                return BadRequest(resultado);

            return Created($"/api/times/{resultado.Data?.Id}", resultado);
        }

        /// <summary>
        /// Atualiza nome/descrição de um time existente.
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Atualizar(string id, [FromBody] AtualizarTimeRequest request)
        {
            var resultado = _atualizarService.Executar(id, request);

            if (!resultado.Success)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        /// <summary>
        /// Exclui um time (somente se não houver objetivos associados).
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
