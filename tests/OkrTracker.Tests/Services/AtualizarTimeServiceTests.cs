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
    /// Testes unitários para AtualizarProjetoService.
    /// </summary>
    public class AtualizarTimeServiceTests
    {
        private readonly Mock<IProjetoRepository> _projetoRepoMock;
        private readonly Mock<ILogger<AtualizarProjetoService>> _loggerMock;
        private readonly AtualizarProjetoService _service;

        public AtualizarTimeServiceTests()
        {
            _projetoRepoMock = new Mock<IProjetoRepository>();
            _loggerMock = new Mock<ILogger<AtualizarProjetoService>>();
            _service = new AtualizarProjetoService(_projetoRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Executar_DadosValidos_DeveRetornarSucesso()
        {
            // Arrange
            var projeto = new Projeto { Id = "1", Nome = "Bridge", DataCriacao = DateTime.UtcNow };
            _projetoRepoMock.Setup(r => r.ObterPorId("1")).Returns(projeto);
            _projetoRepoMock.Setup(r => r.ObterPorNome("Platform")).Returns((Projeto?)null);

            // Act
            var resultado = _service.Executar("1", new AtualizarProjetoRequest { Nome = "Platform", Descricao = "Nova desc" });

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data!.Nome.Should().Be("Platform");
            resultado.Data.Descricao.Should().Be("Nova desc");
        }

        [Fact]
        public void Executar_NomeVazio_DeveRetornarErro()
        {
            // Act
            var resultado = _service.Executar("1", new AtualizarProjetoRequest { Nome = "" });

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("O nome do projeto é obrigatório.");
        }

        [Fact]
        public void Executar_TimeNaoEncontrado_DeveRetornarErro()
        {
            // Arrange
            _projetoRepoMock.Setup(r => r.ObterPorId("999")).Returns((Projeto?)null);

            // Act
            var resultado = _service.Executar("999", new AtualizarProjetoRequest { Nome = "Platform" });

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Projeto não encontrado.");
        }

        [Fact]
        public void Executar_NomeDuplicadoOutroId_DeveRetornarErro()
        {
            // Arrange
            var projeto = new Projeto { Id = "1", Nome = "Bridge" };
            var outroProjeto = new Projeto { Id = "2", Nome = "Platform" };
            _projetoRepoMock.Setup(r => r.ObterPorId("1")).Returns(projeto);
            _projetoRepoMock.Setup(r => r.ObterPorNome("Platform")).Returns(outroProjeto);

            // Act
            var resultado = _service.Executar("1", new AtualizarProjetoRequest { Nome = "Platform" });

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Já existe um projeto com este nome.");
        }
    }
}
