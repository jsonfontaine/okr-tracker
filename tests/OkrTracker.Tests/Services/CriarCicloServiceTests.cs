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
    /// Testes unitários para CriarCicloService.
    /// </summary>
    public class CriarCicloServiceTests
    {
        private readonly Mock<ICicloRepository> _cicloRepoMock;
        private readonly Mock<ILogger<CriarCicloService>> _loggerMock;
        private readonly CriarCicloService _service;

        public CriarCicloServiceTests()
        {
            _cicloRepoMock = new Mock<ICicloRepository>();
            _loggerMock = new Mock<ILogger<CriarCicloService>>();
            _service = new CriarCicloService(_cicloRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Executar_NomeValido_DeveRetornarSucesso()
        {
            // Arrange
            _cicloRepoMock.Setup(r => r.ObterPorNome("2026-Q1")).Returns((Ciclo?)null);

            // Act
            var resultado = _service.Executar(new CriarCicloRequest { Nome = "2026-Q1" });

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data.Should().NotBeNull();
            resultado.Data!.Nome.Should().Be("2026-Q1");
            resultado.Data.Id.Should().NotBeNullOrEmpty();
            _cicloRepoMock.Verify(r => r.Inserir(It.IsAny<Ciclo>()), Times.Once);
        }

        [Fact]
        public void Executar_NomeVazio_DeveRetornarErro()
        {
            // Act
            var resultado = _service.Executar(new CriarCicloRequest { Nome = "" });

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("O nome do ciclo é obrigatório.");
        }

        [Fact]
        public void Executar_NomeDuplicado_DeveRetornarErro()
        {
            // Arrange
            _cicloRepoMock.Setup(r => r.ObterPorNome("2026-Q1")).Returns(new Ciclo { Id = "1", Nome = "2026-Q1" });

            // Act
            var resultado = _service.Executar(new CriarCicloRequest { Nome = "2026-Q1" });

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Já existe um ciclo com este nome.");
        }
    }
}
