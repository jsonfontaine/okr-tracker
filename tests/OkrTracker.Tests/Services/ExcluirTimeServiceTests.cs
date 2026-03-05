using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OkrTracker.Application.Services;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Tests.Services
{
    /// <summary>
    /// Testes unitários para ExcluirTimeService.
    /// </summary>
    public class ExcluirTimeServiceTests
    {
        private readonly Mock<ITimeRepository> _timeRepoMock;
        private readonly Mock<IObjetivoRepository> _objetivoRepoMock;
        private readonly Mock<ILogger<ExcluirTimeService>> _loggerMock;
        private readonly ExcluirTimeService _service;

        public ExcluirTimeServiceTests()
        {
            _timeRepoMock = new Mock<ITimeRepository>();
            _objetivoRepoMock = new Mock<IObjetivoRepository>();
            _loggerMock = new Mock<ILogger<ExcluirTimeService>>();
            _service = new ExcluirTimeService(_timeRepoMock.Object, _objetivoRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Executar_TimeSemObjetivos_DeveRetornarSucesso()
        {
            // Arrange
            _timeRepoMock.Setup(r => r.ObterPorId("1")).Returns(new Time { Id = "1", Nome = "Bridge" });
            _objetivoRepoMock.Setup(r => r.ExistemObjetivosParaTime("1")).Returns(false);

            // Act
            var resultado = _service.Executar("1");

            // Assert
            resultado.Success.Should().BeTrue();
            _timeRepoMock.Verify(r => r.Excluir("1"), Times.Once);
        }

        [Fact]
        public void Executar_TimeComObjetivos_DeveRetornarErro()
        {
            // Arrange
            _timeRepoMock.Setup(r => r.ObterPorId("1")).Returns(new Time { Id = "1", Nome = "Bridge" });
            _objetivoRepoMock.Setup(r => r.ExistemObjetivosParaTime("1")).Returns(true);

            // Act
            var resultado = _service.Executar("1");

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Não é possível excluir o time pois existem objetivos associados.");
        }

        [Fact]
        public void Executar_TimeNaoEncontrado_DeveRetornarErro()
        {
            // Arrange
            _timeRepoMock.Setup(r => r.ObterPorId("999")).Returns((Time?)null);

            // Act
            var resultado = _service.Executar("999");

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Time não encontrado.");
        }
    }
}
