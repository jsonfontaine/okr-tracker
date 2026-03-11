using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OkrTracker.Application.Services;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Tests.Services
{
    /// <summary>
    /// Testes unitários para ListarTimesService.
    /// </summary>
    public class ListarTimesServiceTests
    {
        private readonly Mock<IProjetoRepository> _projetoRepoMock;
        private readonly Mock<ILogger<ListarProjetosService>> _loggerMock;
        private readonly ListarProjetosService _service;

        public ListarTimesServiceTests()
        {
            _projetoRepoMock = new Mock<IProjetoRepository>();
            _loggerMock = new Mock<ILogger<ListarProjetosService>>();
            _service = new ListarProjetosService(_projetoRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Executar_ComProjetos_DeveRetornarLista()
        {
            // Arrange
            var projetos = new List<Projeto>
            {
                new() { Id = "1", Nome = "Bridge" },
                new() { Id = "2", Nome = "Platform" }
            };
            _projetoRepoMock.Setup(r => r.ObterTodos()).Returns(projetos);

            // Act
            var resultado = _service.Executar();

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data.Should().HaveCount(2);
        }

        [Fact]
        public void Executar_SemProjetos_DeveRetornarListaVazia()
        {
            // Arrange
            _projetoRepoMock.Setup(r => r.ObterTodos()).Returns(new List<Projeto>());

            // Act
            var resultado = _service.Executar();

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data.Should().BeEmpty();
        }
    }
}
