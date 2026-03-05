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
    /// Testes unitários para AtualizarTimeService.
    /// </summary>
    public class AtualizarTimeServiceTests
    {
        private readonly Mock<ITimeRepository> _timeRepoMock;
        private readonly Mock<ILogger<AtualizarTimeService>> _loggerMock;
        private readonly AtualizarTimeService _service;

        public AtualizarTimeServiceTests()
        {
            _timeRepoMock = new Mock<ITimeRepository>();
            _loggerMock = new Mock<ILogger<AtualizarTimeService>>();
            _service = new AtualizarTimeService(_timeRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Executar_DadosValidos_DeveRetornarSucesso()
        {
            // Arrange
            var time = new Time { Id = "1", Nome = "Bridge", DataCriacao = DateTime.UtcNow };
            _timeRepoMock.Setup(r => r.ObterPorId("1")).Returns(time);
            _timeRepoMock.Setup(r => r.ObterPorNome("Platform")).Returns((Time?)null);

            // Act
            var resultado = _service.Executar("1", new AtualizarTimeRequest { Nome = "Platform", Descricao = "Nova desc" });

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data!.Nome.Should().Be("Platform");
            resultado.Data.Descricao.Should().Be("Nova desc");
        }

        [Fact]
        public void Executar_NomeVazio_DeveRetornarErro()
        {
            // Act
            var resultado = _service.Executar("1", new AtualizarTimeRequest { Nome = "" });

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("O nome do time é obrigatório.");
        }

        [Fact]
        public void Executar_TimeNaoEncontrado_DeveRetornarErro()
        {
            // Arrange
            _timeRepoMock.Setup(r => r.ObterPorId("999")).Returns((Time?)null);

            // Act
            var resultado = _service.Executar("999", new AtualizarTimeRequest { Nome = "Platform" });

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Time não encontrado.");
        }

        [Fact]
        public void Executar_NomeDuplicadoOutroId_DeveRetornarErro()
        {
            // Arrange
            var time = new Time { Id = "1", Nome = "Bridge" };
            var outroTime = new Time { Id = "2", Nome = "Platform" };
            _timeRepoMock.Setup(r => r.ObterPorId("1")).Returns(time);
            _timeRepoMock.Setup(r => r.ObterPorNome("Platform")).Returns(outroTime);

            // Act
            var resultado = _service.Executar("1", new AtualizarTimeRequest { Nome = "Platform" });

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Já existe um time com este nome.");
        }
    }
}
