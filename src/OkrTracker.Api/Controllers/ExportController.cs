using Microsoft.AspNetCore.Mvc;
using OkrTracker.Application.Interfaces;

namespace OkrTracker.Api.Controllers
{
    /// <summary>
    /// Controller para exportação do resumo executivo dos OKRs.
    /// </summary>
    [ApiController]
    [Route("api/export")]
    public class ExportController : ControllerBase
    {
        private readonly IExportarResumoExecutivoService _exportarService;

        public ExportController(IExportarResumoExecutivoService exportarService)
        {
            _exportarService = exportarService;
        }

        /// <summary>
        /// Gera o resumo executivo em texto para um determinado projeto e ciclo.
        /// </summary>
        [HttpGet("resumo-executivo")]
        public IActionResult ExportarResumoExecutivo([FromQuery] string cicloId, [FromQuery] string projetoId)
        {
            var resultado = _exportarService.Executar(cicloId, projetoId);

            if (!resultado.Success)
                return BadRequest(resultado);

            return Ok(resultado);
        }
    }
}
