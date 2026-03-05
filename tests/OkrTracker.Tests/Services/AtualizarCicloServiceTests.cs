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
    /// Testes unitários para AtualizarCicloService.
    /// </summary>
    public class AtualizarCicloServiceTests
    {
        private readonly Mock<ICicloRepository> _cicloRepoMock;
        private readonly Mock<ILogger<AtualizarCicloService>> _loggerMock;
        private readonly AtualizarCicloService _service;

        public AtualizarCicloServiceTests()
        {
            _cicloRepoMock = new Mock<ICicloRepository>();
            _loggerMock = new Mock<ILogger<AtualizarCicloService>>();
            _service = new AtualizarCicloService(_cicloRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Executar_DadosValidos_DeveRetornarSucesso()
        {
            // Arrange
            var ciclo = new Ciclo { Id = "1", Nome = "2026-Q1", DataCriacao = DateTime.UtcNow };
            _cicloRepoMock.Setup(r => r.ObterPorId("1")).Returns(ciclo);
            _cicloRepoMock.Setup(r => r.ObterPorNome("2026-Q2")).Returns((Ciclo?)null);

            // Act
            var resultado = _service.Executar("1", new AtualizarCicloRequest { Nome = "2026-Q2" });

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data!.Nome.Should().Be("2026-Q2");
            _cicloRepoMock.Verify(r => r.Atualizar(It.IsAny<Ciclo>()), Times.Once);
        }

        [Fact]
        public void Executar_NomeVazio_DeveRetornarErro()
        {
            // Act
            var resultado = _service.Executar("1", new AtualizarCicloRequest { Nome = "" });

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("O nome do ciclo é obrigatório.");
        }

        [Fact]
        public void Executar_CicloNaoEncontrado_DeveRetornarErro()
        {
            // Arrange
            _cicloRepoMock.Setup(r => r.ObterPorId("999")).Returns((Ciclo?)null);

            // Act
            var resultado = _service.Executar("999", new AtualizarCicloRequest { Nome = "2026-Q2" });

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Ciclo não encontrado.");
        }

        [Fact]
        public void Executar_NomeDuplicadoOutroId_DeveRetornarErro()
        {
            // Arrange
            var ciclo = new Ciclo { Id = "1", Nome = "2026-Q1" };
            var outroCiclo = new Ciclo { Id = "2", Nome = "2026-Q2" };
            _cicloRepoMock.Setup(r => r.ObterPorId("1")).Returns(ciclo);
            _cicloRepoMock.Setup(r => r.ObterPorNome("2026-Q2")).Returns(outroCiclo);

            // Act
            var resultado = _service.Executar("1", new AtualizarCicloRequest { Nome = "2026-Q2" });

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Já existe um ciclo com este nome.");
        }

        [Fact]
        public void Executar_MesmoNomeMesmoId_DeveRetornarSucesso()
        {
            // Arrange
            var ciclo = new Ciclo { Id = "1", Nome = "2026-Q1", DataCriacao = DateTime.UtcNow };
            _cicloRepoMock.Setup(r => r.ObterPorId("1")).Returns(ciclo);
            _cicloRepoMock.Setup(r => r.ObterPorNome("2026-Q1")).Returns(ciclo);

            // Act
            var resultado = _service.Executar("1", new AtualizarCicloRequest { Nome = "2026-Q1" });

            // Assert
            resultado.Success.Should().BeTrue();
        }
    }
}
