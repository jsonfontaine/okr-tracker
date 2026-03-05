using Microsoft.AspNetCore.Mvc;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;

namespace OkrTracker.Api.Controllers
{
    /// <summary>
    /// Controller para operações de Fatos Relevantes.
    /// </summary>
    [ApiController]
    [Route("api/fatos-relevantes")]
    public class FatosRelevantesController : ControllerBase
    {
        private readonly ICriarFatoRelevanteService _criarService;

        public FatosRelevantesController(ICriarFatoRelevanteService criarService)
        {
            _criarService = criarService;
        }

        /// <summary>
        /// Registra um fato relevante em um objetivo ou KR.
        /// Exatamente um entre objetivoId e krId deve ser preenchido.
        /// </summary>
        [HttpPost]
        public IActionResult Criar([FromBody] CriarFatoRelevanteRequest request)
        {
            var resultado = _criarService.Executar(request);

            if (!resultado.Success)
                return BadRequest(resultado);

            return Created($"/api/fatos-relevantes/{resultado.Data?.Id}", resultado);
        }
    }
}
