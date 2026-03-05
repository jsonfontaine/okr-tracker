using Microsoft.AspNetCore.Mvc;
using OkrTracker.Application.Interfaces;

namespace OkrTracker.Api.Controllers
{
    /// <summary>
    /// Controller para exportação de OKRs em formato Adaptive Card.
    /// </summary>
    [ApiController]
    [Route("api/export")]
    public class ExportController : ControllerBase
    {
        private readonly IExportarAdaptiveCardService _exportarService;

        public ExportController(IExportarAdaptiveCardService exportarService)
        {
            _exportarService = exportarService;
        }

        /// <summary>
        /// Gera o JSON de Adaptive Card para um determinado time e ciclo.
        /// </summary>
        [HttpGet("adaptive-card")]
        public IActionResult ExportarAdaptiveCard([FromQuery] string cicloId, [FromQuery] string timeId)
        {
            var resultado = _exportarService.Executar(cicloId, timeId);

            if (!resultado.Success)
                return BadRequest(resultado);

            return Ok(resultado);
        }
    }
}
