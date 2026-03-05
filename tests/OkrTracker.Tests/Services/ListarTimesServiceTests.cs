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
        private readonly Mock<ITimeRepository> _timeRepoMock;
        private readonly Mock<ILogger<ListarTimesService>> _loggerMock;
        private readonly ListarTimesService _service;

        public ListarTimesServiceTests()
        {
            _timeRepoMock = new Mock<ITimeRepository>();
            _loggerMock = new Mock<ILogger<ListarTimesService>>();
            _service = new ListarTimesService(_timeRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Executar_ComTimes_DeveRetornarLista()
        {
            // Arrange
            var times = new List<Time>
            {
                new() { Id = "1", Nome = "Bridge" },
                new() { Id = "2", Nome = "Platform" }
            };
            _timeRepoMock.Setup(r => r.ObterTodos()).Returns(times);

            // Act
            var resultado = _service.Executar();

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data.Should().HaveCount(2);
        }

        [Fact]
        public void Executar_SemTimes_DeveRetornarListaVazia()
        {
            // Arrange
            _timeRepoMock.Setup(r => r.ObterTodos()).Returns(new List<Time>());

            // Act
            var resultado = _service.Executar();

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data.Should().BeEmpty();
        }
    }
}
