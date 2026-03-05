using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OkrTracker.Application.Services;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Tests.Services
{
    /// <summary>
    /// Testes unitários para ExcluirCicloService.
    /// </summary>
    public class ExcluirCicloServiceTests
    {
        private readonly Mock<ICicloRepository> _cicloRepoMock;
        private readonly Mock<IObjetivoRepository> _objetivoRepoMock;
        private readonly Mock<ILogger<ExcluirCicloService>> _loggerMock;
        private readonly ExcluirCicloService _service;

        public ExcluirCicloServiceTests()
        {
            _cicloRepoMock = new Mock<ICicloRepository>();
            _objetivoRepoMock = new Mock<IObjetivoRepository>();
            _loggerMock = new Mock<ILogger<ExcluirCicloService>>();
            _service = new ExcluirCicloService(_cicloRepoMock.Object, _objetivoRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Executar_CicloSemObjetivos_DeveRetornarSucesso()
        {
            // Arrange
            _cicloRepoMock.Setup(r => r.ObterPorId("1")).Returns(new Ciclo { Id = "1", Nome = "2026-Q1" });
            _objetivoRepoMock.Setup(r => r.ExistemObjetivosParaCiclo("1")).Returns(false);

            // Act
            var resultado = _service.Executar("1");

            // Assert
            resultado.Success.Should().BeTrue();
            _cicloRepoMock.Verify(r => r.Excluir("1"), Times.Once);
        }

        [Fact]
        public void Executar_CicloComObjetivos_DeveRetornarErro()
        {
            // Arrange
            _cicloRepoMock.Setup(r => r.ObterPorId("1")).Returns(new Ciclo { Id = "1", Nome = "2026-Q1" });
            _objetivoRepoMock.Setup(r => r.ExistemObjetivosParaCiclo("1")).Returns(true);

            // Act
            var resultado = _service.Executar("1");

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Não é possível excluir o ciclo pois existem objetivos associados.");
        }

        [Fact]
        public void Executar_CicloNaoEncontrado_DeveRetornarErro()
        {
            // Arrange
            _cicloRepoMock.Setup(r => r.ObterPorId("999")).Returns((Ciclo?)null);

            // Act
            var resultado = _service.Executar("999");

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Ciclo não encontrado.");
        }
    }
}
