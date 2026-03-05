using Microsoft.AspNetCore.Mvc;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;

namespace OkrTracker.Api.Controllers
{
    /// <summary>
    /// Controller para operações de Key Results.
    /// </summary>
    [ApiController]
    [Route("api/krs")]
    public class KeyResultsController : ControllerBase
    {
        private readonly ICriarKRService _criarService;
        private readonly IAtualizarKRService _atualizarService;
        private readonly IAtualizarProgressoKRService _atualizarProgressoService;
        private readonly IExcluirKRService _excluirService;

        public KeyResultsController(
            ICriarKRService criarService,
            IAtualizarKRService atualizarService,
            IAtualizarProgressoKRService atualizarProgressoService,
            IExcluirKRService excluirService)
        {
            _criarService = criarService;
            _atualizarService = atualizarService;
            _atualizarProgressoService = atualizarProgressoService;
            _excluirService = excluirService;
        }

        /// <summary>
        /// Cria um novo Key Result vinculado a um objetivo.
        /// </summary>
        [HttpPost]
        public IActionResult Criar([FromBody] CriarKeyResultRequest request)
        {
            var resultado = _criarService.Executar(request);

            if (!resultado.Success)
                return BadRequest(resultado);

            return Created($"/api/krs/{resultado.Data?.Id}", resultado);
        }

        /// <summary>
        /// Atualiza campos de um Key Result (exceto objetivoId).
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Atualizar(string id, [FromBody] AtualizarKeyResultRequest request)
        {
            var resultado = _atualizarService.Executar(id, request);

            if (!resultado.Success)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        /// <summary>
        /// Atualiza o progresso de um Key Result.
        /// Para tipo Requisito, só aceita 0 ou 100.
        /// </summary>
        [HttpPut("{id}/progresso")]
        public IActionResult AtualizarProgresso(string id, [FromBody] AtualizarProgressoRequest request)
        {
            var resultado = _atualizarProgressoService.Executar(id, request);

            if (!resultado.Success)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        /// <summary>
        /// Exclui um Key Result (impede exclusão se for o último KR do objetivo).
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
