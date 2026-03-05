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
    /// Testes unitários para CriarFatoRelevanteService.
    /// </summary>
    public class CriarFatoRelevanteServiceTests
    {
        private readonly Mock<IFatoRelevanteRepository> _fatoRepoMock;
        private readonly Mock<IObjetivoRepository> _objetivoRepoMock;
        private readonly Mock<IKeyResultRepository> _krRepoMock;
        private readonly Mock<ILogger<CriarFatoRelevanteService>> _loggerMock;
        private readonly CriarFatoRelevanteService _service;

        public CriarFatoRelevanteServiceTests()
        {
            _fatoRepoMock = new Mock<IFatoRelevanteRepository>();
            _objetivoRepoMock = new Mock<IObjetivoRepository>();
            _krRepoMock = new Mock<IKeyResultRepository>();
            _loggerMock = new Mock<ILogger<CriarFatoRelevanteService>>();
            _service = new CriarFatoRelevanteService(
                _fatoRepoMock.Object,
                _objetivoRepoMock.Object,
                _krRepoMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public void Executar_FatoEmObjetivo_DeveRetornarSucesso()
        {
            // Arrange
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(new Objetivo { Id = "obj-1" });
            var request = new CriarFatoRelevanteRequest
            {
                ObjetivoId = "obj-1",
                KrId = null,
                Texto = "Mudança de escopo aprovada."
            };

            // Act
            var resultado = _service.Executar(request);

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data!.Texto.Should().Be("Mudança de escopo aprovada.");
            _fatoRepoMock.Verify(r => r.Inserir(It.IsAny<FatoRelevante>()), Times.Once);
        }

        [Fact]
        public void Executar_FatoEmKR_DeveRetornarSucesso()
        {
            _krRepoMock.Setup(r => r.ObterPorId("kr-1")).Returns(new KeyResult { Id = "kr-1" });
            var request = new CriarFatoRelevanteRequest { KrId = "kr-1", Texto = "Fato no KR." };
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeTrue();
        }

        [Fact]
        public void Executar_TextoVazio_DeveRetornarErro()
        {
            var request = new CriarFatoRelevanteRequest { ObjetivoId = "obj-1", Texto = "" };
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Texto do fato relevante é obrigatório.");
        }

        [Fact]
        public void Executar_AmbosIds_DeveRetornarErro()
        {
            var request = new CriarFatoRelevanteRequest { ObjetivoId = "obj-1", KrId = "kr-1", Texto = "Texto" };
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Informe exatamente um entre ObjetivoId e KrId.");
        }

        [Fact]
        public void Executar_NenhumId_DeveRetornarErro()
        {
            var request = new CriarFatoRelevanteRequest { Texto = "Texto" };
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Informe exatamente um entre ObjetivoId e KrId.");
        }

        [Fact]
        public void Executar_ObjetivoNaoEncontrado_DeveRetornarErro()
        {
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-999")).Returns((Objetivo?)null);
            var request = new CriarFatoRelevanteRequest { ObjetivoId = "obj-999", Texto = "Texto" };
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Objetivo não encontrado.");
        }

        [Fact]
        public void Executar_KRNaoEncontrado_DeveRetornarErro()
        {
            _krRepoMock.Setup(r => r.ObterPorId("kr-999")).Returns((KeyResult?)null);
            var request = new CriarFatoRelevanteRequest { KrId = "kr-999", Texto = "Texto" };
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("KR não encontrado.");
        }
    }
}
