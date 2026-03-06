using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OkrTracker.Application.Services;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Tests.Services
{
    /// <summary>
    /// Testes unitários para ExcluirKRService.
    /// </summary>
    public class ExcluirKRServiceTests
    {
        private readonly Mock<IKeyResultRepository> _krRepoMock;
        private readonly Mock<IObjetivoRepository> _objetivoRepoMock;
        private readonly Mock<ILogger<ExcluirKRService>> _loggerMock;
        private readonly ExcluirKRService _service;

        public ExcluirKRServiceTests()
        {
            _krRepoMock = new Mock<IKeyResultRepository>();
            _objetivoRepoMock = new Mock<IObjetivoRepository>();
            _loggerMock = new Mock<ILogger<ExcluirKRService>>();
            _service = new ExcluirKRService(_krRepoMock.Object, _objetivoRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Executar_KRComMaisDeUm_DeveRetornarSucesso()
        {
            // Arrange
            var kr = new KeyResult { Id = "kr-1", ObjetivoId = "obj-1", Progresso = 50 };
            _krRepoMock.Setup(r => r.ObterPorId("kr-1")).Returns(kr);
            _krRepoMock.Setup(r => r.ContarPorObjetivoId("obj-1")).Returns(2);

            // Act
            var resultado = _service.Executar("kr-1");

            // Assert
            resultado.Success.Should().BeTrue();
            _krRepoMock.Verify(r => r.Excluir("kr-1"), Times.Once);
        }

        [Fact]
        public void Executar_UltimoKRDoObjetivo_DeveRetornarErro()
        {
            // Arrange
            var kr = new KeyResult { Id = "kr-1", ObjetivoId = "obj-1" };
            _krRepoMock.Setup(r => r.ObterPorId("kr-1")).Returns(kr);
            _krRepoMock.Setup(r => r.ContarPorObjetivoId("obj-1")).Returns(1);

            // Act
            var resultado = _service.Executar("kr-1");

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Não é possível excluir o último KR de um objetivo.");
        }

        [Fact]
        public void Executar_KRNaoEncontrado_DeveRetornarErro()
        {
            _krRepoMock.Setup(r => r.ObterPorId("kr-999")).Returns((KeyResult?)null);
            var resultado = _service.Executar("kr-999");
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("KR não encontrado.");
        }

        [Fact]
        public void Executar_NaoDeveRecalcularProgressoDoObjetivoAposExclusao()
        {
            // Arrange
            var kr = new KeyResult { Id = "kr-1", ObjetivoId = "obj-1", Progresso = 20 };
            _krRepoMock.Setup(r => r.ObterPorId("kr-1")).Returns(kr);
            _krRepoMock.Setup(r => r.ContarPorObjetivoId("obj-1")).Returns(3);

            // Act
            var resultado = _service.Executar("kr-1");

            // Assert
            resultado.Success.Should().BeTrue();
            _objetivoRepoMock.Verify(r => r.Atualizar(It.IsAny<Objetivo>()), Times.Never);
        }
    }
}
