using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Services;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Tests.Services
{
    /// <summary>
    /// Testes unitários para CriarComentarioService.
    /// </summary>
    public class CriarComentarioServiceTests
    {
        private readonly Mock<IComentarioRepository> _comentarioRepoMock;
        private readonly Mock<IObjetivoRepository> _objetivoRepoMock;
        private readonly Mock<IKeyResultRepository> _krRepoMock;
        private readonly Mock<ILogger<CriarComentarioService>> _loggerMock;
        private readonly CriarComentarioService _service;

        public CriarComentarioServiceTests()
        {
            _comentarioRepoMock = new Mock<IComentarioRepository>();
            _objetivoRepoMock = new Mock<IObjetivoRepository>();
            _krRepoMock = new Mock<IKeyResultRepository>();
            _loggerMock = new Mock<ILogger<CriarComentarioService>>();
            _service = new CriarComentarioService(
                _comentarioRepoMock.Object,
                _objetivoRepoMock.Object,
                _krRepoMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public void Executar_ComentarioEmObjetivo_DeveRetornarSucesso()
        {
            // Arrange
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(new Objetivo { Id = "obj-1" });

            var request = new CriarComentarioRequest
            {
                ObjetivoId = "obj-1",
                KrId = null,
                Texto = "Reunião com o time."
            };

            // Act
            var resultado = _service.Executar(request);

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data!.Texto.Should().Be("Reunião com o time.");
            resultado.Data.ObjetivoId.Should().Be("obj-1");
            resultado.Data.KrId.Should().BeNull();
            _comentarioRepoMock.Verify(r => r.Inserir(It.IsAny<Comentario>()), Times.Once);
        }

        [Fact]
        public void Executar_ComentarioEmKR_DeveRetornarSucesso()
        {
            // Arrange
            _krRepoMock.Setup(r => r.ObterPorId("kr-1")).Returns(new KeyResult { Id = "kr-1" });

            var request = new CriarComentarioRequest
            {
                ObjetivoId = null,
                KrId = "kr-1",
                Texto = "Progresso atualizado."
            };

            // Act
            var resultado = _service.Executar(request);

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data!.KrId.Should().Be("kr-1");
        }

        [Fact]
        public void Executar_TextoVazio_DeveRetornarErro()
        {
            var request = new CriarComentarioRequest { ObjetivoId = "obj-1", Texto = "" };
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Texto do comentário é obrigatório.");
        }

        [Fact]
        public void Executar_AmbosObjetivoIdEKrIdPreenchidos_DeveRetornarErro()
        {
            var request = new CriarComentarioRequest
            {
                ObjetivoId = "obj-1",
                KrId = "kr-1",
                Texto = "Texto"
            };
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Informe exatamente um entre ObjetivoId e KrId.");
        }

        [Fact]
        public void Executar_NenhumObjetivoIdOuKrId_DeveRetornarErro()
        {
            var request = new CriarComentarioRequest
            {
                ObjetivoId = null,
                KrId = null,
                Texto = "Texto"
            };
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Informe exatamente um entre ObjetivoId e KrId.");
        }

        [Fact]
        public void Executar_ObjetivoNaoEncontrado_DeveRetornarErro()
        {
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-999")).Returns((Objetivo?)null);
            var request = new CriarComentarioRequest { ObjetivoId = "obj-999", Texto = "Texto" };
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Objetivo não encontrado.");
        }

        [Fact]
        public void Executar_KRNaoEncontrado_DeveRetornarErro()
        {
            _krRepoMock.Setup(r => r.ObterPorId("kr-999")).Returns((KeyResult?)null);
            var request = new CriarComentarioRequest { KrId = "kr-999", Texto = "Texto" };
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("KR não encontrado.");
        }
    }
}
