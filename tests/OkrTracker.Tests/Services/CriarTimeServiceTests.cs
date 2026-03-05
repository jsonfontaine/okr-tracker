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
    /// Testes unitários para CriarTimeService.
    /// </summary>
    public class CriarTimeServiceTests
    {
        private readonly Mock<ITimeRepository> _timeRepoMock;
        private readonly Mock<ILogger<CriarTimeService>> _loggerMock;
        private readonly CriarTimeService _service;

        public CriarTimeServiceTests()
        {
            _timeRepoMock = new Mock<ITimeRepository>();
            _loggerMock = new Mock<ILogger<CriarTimeService>>();
            _service = new CriarTimeService(_timeRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Executar_NomeValido_DeveRetornarSucesso()
        {
            // Arrange
            _timeRepoMock.Setup(r => r.ObterPorNome("Bridge")).Returns((Time?)null);

            // Act
            var resultado = _service.Executar(new CriarTimeRequest { Nome = "Bridge", Descricao = "Time Bridge" });

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data!.Nome.Should().Be("Bridge");
            resultado.Data.Descricao.Should().Be("Time Bridge");
            _timeRepoMock.Verify(r => r.Inserir(It.IsAny<Time>()), Times.Once);
        }

        [Fact]
        public void Executar_NomeVazio_DeveRetornarErro()
        {
            // Act
            var resultado = _service.Executar(new CriarTimeRequest { Nome = "" });

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("O nome do time é obrigatório.");
        }

        [Fact]
        public void Executar_NomeDuplicado_DeveRetornarErro()
        {
            // Arrange
            _timeRepoMock.Setup(r => r.ObterPorNome("Bridge")).Returns(new Time { Id = "1", Nome = "Bridge" });

            // Act
            var resultado = _service.Executar(new CriarTimeRequest { Nome = "Bridge" });

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Já existe um time com este nome.");
        }
    }
}
