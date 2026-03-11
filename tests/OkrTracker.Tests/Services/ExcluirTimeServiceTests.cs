using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OkrTracker.Application.Services;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Tests.Services
{
    /// <summary>
    /// Testes unitários para ExcluirProjetoService.
    /// </summary>
    public class ExcluirProjetoServiceTests
    {
        private readonly Mock<IProjetoRepository> _projetoRepoMock;
        private readonly Mock<IObjetivoRepository> _objetivoRepoMock;
        private readonly Mock<ILogger<ExcluirProjetoService>> _loggerMock;
        private readonly ExcluirProjetoService _service;

        public ExcluirProjetoServiceTests()
        {
            _projetoRepoMock = new Mock<IProjetoRepository>();
            _objetivoRepoMock = new Mock<IObjetivoRepository>();
            _loggerMock = new Mock<ILogger<ExcluirProjetoService>>();
            _service = new ExcluirProjetoService(_projetoRepoMock.Object, _objetivoRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Executar_TimeSemObjetivos_DeveRetornarSucesso()
        {
            // Arrange
            _projetoRepoMock.Setup(r => r.ObterPorId("1")).Returns(new Projeto { Id = "1", Nome = "Bridge" });
            _objetivoRepoMock.Setup(r => r.ExistemObjetivosParaProjeto("1")).Returns(false);

            // Act
            var resultado = _service.Executar("1");

            // Assert
            resultado.Success.Should().BeTrue();
            _projetoRepoMock.Verify(r => r.Excluir("1"), Times.Once);
        }

        [Fact]
        public void Executar_TimeComObjetivos_DeveRetornarErro()
        {
            // Arrange
            _projetoRepoMock.Setup(r => r.ObterPorId("1")).Returns(new Projeto { Id = "1", Nome = "Bridge" });
            _objetivoRepoMock.Setup(r => r.ExistemObjetivosParaProjeto("1")).Returns(true);

            // Act
            var resultado = _service.Executar("1");

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Não é possível excluir o projeto pois existem objetivos associados.");
        }

        [Fact]
        public void Executar_TimeNaoEncontrado_DeveRetornarErro()
        {
            // Arrange
            _projetoRepoMock.Setup(r => r.ObterPorId("999")).Returns((Projeto?)null);

            // Act
            var resultado = _service.Executar("999");

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Projeto não encontrado.");
        }
    }
}
