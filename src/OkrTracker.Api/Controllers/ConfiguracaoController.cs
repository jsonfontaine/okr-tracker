using Microsoft.AspNetCore.Mvc;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;

namespace OkrTracker.Api.Controllers
{
    /// <summary>
    /// Controller para configuração da base de dados LiteDB.
    /// </summary>
    [ApiController]
    [Route("api/config")]
    public class ConfiguracaoController : ControllerBase
    {
        private readonly IConfigurarBaseDeDadosService _configurarService;

        public ConfiguracaoController(IConfigurarBaseDeDadosService configurarService)
        {
            _configurarService = configurarService;
        }

        /// <summary>
        /// Configura o caminho do arquivo .db do LiteDB.
        /// </summary>
        [HttpPost("database")]
        public IActionResult ConfigurarDatabase([FromBody] ConfigurarDatabaseRequest request)
        {
            var resultado = _configurarService.Configurar(request.DatabasePath);

            if (!resultado.Success)
                return BadRequest(resultado);

            return Ok(resultado);
        }
    }
}
