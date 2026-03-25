using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OkrTracker.Application.Services;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Tests.Services
{
    /// <summary>
    /// Testes unitários para ListarCiclosService.
    /// </summary>
    public class ListarCiclosServiceTests
    {
        private readonly Mock<ICicloRepository> _cicloRepoMock;
        private readonly Mock<ILogger<ListarCiclosService>> _loggerMock;
        private readonly ListarCiclosService _service;

        public ListarCiclosServiceTests()
        {
            _cicloRepoMock = new Mock<ICicloRepository>();
            _loggerMock = new Mock<ILogger<ListarCiclosService>>();
            _service = new ListarCiclosService(_cicloRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Executar_ComCiclos_DeveRetornarLista()
        {
            // Arrange
            var ciclos = new List<Ciclo>
            {
                new() { Id = "1", Nome = "2026-Q1", DataCriacao = DateTime.UtcNow },
                new() { Id = "2", Nome = "2026-Q2", DataCriacao = DateTime.UtcNow }
            };
            _cicloRepoMock.Setup(r => r.ObterTodos()).Returns(ciclos);

            // Act
            var resultado = _service.Executar();

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data.Should().HaveCount(2);
        }

        [Fact]
        public void Executar_SemCiclos_DeveRetornarListaVazia()
        {
            // Arrange
            _cicloRepoMock.Setup(r => r.ObterTodos()).Returns(new List<Ciclo>());

            // Act
            var resultado = _service.Executar();

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data.Should().BeEmpty();
        }

        [Fact]
        public void Executar_ComDatas_DeveOrdenarCiclosEmOrdemCrescente()
        {
            // Arrange
            var ciclos = new List<Ciclo>
            {
                new() { Id = "3", Nome = "SemDataInicio", DataInicio = null, DataCriacao = new DateTime(2026, 01, 15, 0, 0, 0, DateTimeKind.Utc) },
                new() { Id = "1", Nome = "2026-Q2", DataInicio = new DateTime(2026, 04, 01, 0, 0, 0, DateTimeKind.Utc), DataCriacao = new DateTime(2026, 03, 01, 0, 0, 0, DateTimeKind.Utc) },
                new() { Id = "2", Nome = "2026-Q1", DataInicio = new DateTime(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc), DataCriacao = new DateTime(2025, 12, 01, 0, 0, 0, DateTimeKind.Utc) }
            };

            _cicloRepoMock.Setup(r => r.ObterTodos()).Returns(ciclos);

            // Act
            var resultado = _service.Executar();

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data.Should().NotBeNull();
            resultado.Data!.Select(c => c.Id).Should().ContainInOrder("2", "3", "1");
        }
    }
}
