using Microsoft.AspNetCore.Mvc;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Application.Ports;

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
        private readonly IDatabasePathProvider _pathProvider;

        public ConfiguracaoController(
            IConfigurarBaseDeDadosService configurarService,
            IDatabasePathProvider pathProvider)
        {
            _configurarService = configurarService;
            _pathProvider = pathProvider;
        }

        /// <summary>
        /// Retorna o status atual da configuração da base de dados.
        /// </summary>
        [HttpGet("database")]
        public IActionResult ObterStatus()
        {
            return Ok(new
            {
                configurado = _pathProvider.EstaConfigurado(),
                caminho = _pathProvider.ObterCaminho()
            });
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
