using Microsoft.AspNetCore.Mvc;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;

namespace OkrTracker.Api.Controllers
{
    /// <summary>
    /// Controller para operações de Riscos.
    /// </summary>
    [ApiController]
    [Route("api/riscos")]
    public class RiscosController : ControllerBase
    {
        private readonly ICriarRiscoService _criarService;

        public RiscosController(ICriarRiscoService criarService)
        {
            _criarService = criarService;
        }

        /// <summary>
        /// Registra um risco em um objetivo ou KR.
        /// Exatamente um entre objetivoId e krId deve ser preenchido.
        /// </summary>
        [HttpPost]
        public IActionResult Criar([FromBody] CriarRiscoRequest request)
        {
            var resultado = _criarService.Executar(request);

            if (!resultado.Success)
                return BadRequest(resultado);

            return Created($"/api/riscos/{resultado.Data?.Id}", resultado);
        }
    }
}
