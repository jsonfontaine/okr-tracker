using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OkrTracker.Application.Services;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Enums;
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
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(new Objetivo { Id = "obj-1" });
            _krRepoMock.Setup(r => r.ObterPorObjetivoId("obj-1")).Returns(new List<KeyResult>
            {
                new() { Id = "kr-2", Progresso = 80 }
            });

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
        public void Executar_DeveRecalcularProgressoDoObjetivoAposExclusao()
        {
            // Arrange
            var kr = new KeyResult { Id = "kr-1", ObjetivoId = "obj-1", Progresso = 20 };
            _krRepoMock.Setup(r => r.ObterPorId("kr-1")).Returns(kr);
            _krRepoMock.Setup(r => r.ContarPorObjetivoId("obj-1")).Returns(3);
            var objetivo = new Objetivo { Id = "obj-1" };
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(objetivo);

            // Após exclusão do kr-1, restam kr-2(60) e kr-3(100) => média 80
            _krRepoMock.Setup(r => r.ObterPorObjetivoId("obj-1")).Returns(new List<KeyResult>
            {
                new() { Id = "kr-2", Progresso = 60 },
                new() { Id = "kr-3", Progresso = 100 }
            });

            // Act
            var resultado = _service.Executar("kr-1");

            // Assert
            resultado.Success.Should().BeTrue();
            _objetivoRepoMock.Verify(r => r.Atualizar(It.Is<Objetivo>(o => o.Progresso == 80)), Times.Once);
        }
    }
}
