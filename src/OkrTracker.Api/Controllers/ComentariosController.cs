using Microsoft.AspNetCore.Mvc;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;

namespace OkrTracker.Api.Controllers
{
    /// <summary>
    /// Controller para operações de Comentários (check-ins).
    /// </summary>
    [ApiController]
    [Route("api/comentarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly ICriarComentarioService _criarService;

        public ComentariosController(ICriarComentarioService criarService)
        {
            _criarService = criarService;
        }

        /// <summary>
        /// Registra um comentário (check-in) em um objetivo ou KR.
        /// Exatamente um entre objetivoId e krId deve ser preenchido.
        /// </summary>
        [HttpPost]
        public IActionResult Criar([FromBody] CriarComentarioRequest request)
        {
            var resultado = _criarService.Executar(request);

            if (!resultado.Success)
                return BadRequest(resultado);

            return Created($"/api/comentarios/{resultado.Data?.Id}", resultado);
        }
    }
}
