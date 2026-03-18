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
    /// Testes unitários para CriarProjetoService.
    /// </summary>
    public class CriarProjetoServiceTests
    {
        private readonly Mock<IProjetoRepository> _projetoRepoMock;
        private readonly Mock<ILogger<CriarProjetoService>> _loggerMock;
        private readonly CriarProjetoService _service;

        public CriarProjetoServiceTests()
        {
            _projetoRepoMock = new Mock<IProjetoRepository>();
            _loggerMock = new Mock<ILogger<CriarProjetoService>>();
            _service = new CriarProjetoService(_projetoRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Executar_NomeValido_DeveRetornarSucesso()
        {
            // Arrange
            _projetoRepoMock.Setup(r => r.ObterPorNome("Bridge")).Returns((Projeto?)null);

            // Act
            var resultado = _service.Executar(new CriarProjetoRequest { Nome = "Bridge", Descricao = "Time Bridge" });

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data!.Nome.Should().Be("Bridge");
            resultado.Data.Descricao.Should().Be("Time Bridge");
            _projetoRepoMock.Verify(r => r.Inserir(It.IsAny<Projeto>()), Times.Once);
        }

        [Fact]
        public void Executar_NomeVazio_DeveRetornarErro()
        {
            // Act
            var resultado = _service.Executar(new CriarProjetoRequest { Nome = "" });

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("O nome do projeto é obrigatório.");
        }

        [Fact]
        public void Executar_NomeDuplicado_DeveRetornarErro()
        {
            // Arrange
            _projetoRepoMock.Setup(r => r.ObterPorNome("Bridge")).Returns(new Projeto { Id = "1", Nome = "Bridge" });

            // Act
            var resultado = _service.Executar(new CriarProjetoRequest { Nome = "Bridge" });

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Já existe um projeto com este nome.");
        }
    }
}
