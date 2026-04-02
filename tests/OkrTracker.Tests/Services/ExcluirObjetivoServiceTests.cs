using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OkrTracker.Application.Services;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Tests.Services
{
    /// <summary>
    /// Testes unitários para ExcluirObjetivoService.
    /// </summary>
    public class ExcluirObjetivoServiceTests
    {
        private readonly Mock<IObjetivoRepository> _objetivoRepoMock;
        private readonly Mock<IKeyResultRepository> _krRepoMock;
        private readonly Mock<IComentarioRepository> _comentarioRepoMock;
        private readonly Mock<IFatoRelevanteRepository> _fatoRepoMock;
        private readonly Mock<IRiscoRepository> _riscoRepoMock;
        private readonly Mock<ILogger<ExcluirObjetivoService>> _loggerMock;
        private readonly ExcluirObjetivoService _service;

        public ExcluirObjetivoServiceTests()
        {
            _objetivoRepoMock = new Mock<IObjetivoRepository>();
            _krRepoMock = new Mock<IKeyResultRepository>();
            _comentarioRepoMock = new Mock<IComentarioRepository>();
            _fatoRepoMock = new Mock<IFatoRelevanteRepository>();
            _riscoRepoMock = new Mock<IRiscoRepository>();
            _loggerMock = new Mock<ILogger<ExcluirObjetivoService>>();

            _service = new ExcluirObjetivoService(
                _objetivoRepoMock.Object,
                _krRepoMock.Object,
                _comentarioRepoMock.Object,
                _fatoRepoMock.Object,
                _riscoRepoMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public void Executar_ObjetivoExistente_DeveRetornarSucessoEExcluirEmCascata()
        {
            // Arrange
            var objetivo = new Objetivo { Id = "obj-1", Titulo = "Objetivo 1" };
            var krs = new List<KeyResult>
            {
                new KeyResult { Id = "kr-1", ObjetivoId = "obj-1" },
                new KeyResult { Id = "kr-2", ObjetivoId = "obj-1" },
            };

            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(objetivo);
            _krRepoMock.Setup(r => r.ObterPorObjetivoId("obj-1")).Returns(krs);

            // Act
            var resultado = _service.Executar("obj-1");

            // Assert
            resultado.Success.Should().BeTrue();

            // Verifica exclusão em cascata para cada KR
            _comentarioRepoMock.Verify(r => r.ExcluirPorKrId("kr-1"), Times.Once);
            _comentarioRepoMock.Verify(r => r.ExcluirPorKrId("kr-2"), Times.Once);
            _fatoRepoMock.Verify(r => r.ExcluirPorKrId("kr-1"), Times.Once);
            _fatoRepoMock.Verify(r => r.ExcluirPorKrId("kr-2"), Times.Once);
            _riscoRepoMock.Verify(r => r.ExcluirPorKrId("kr-1"), Times.Once);
            _riscoRepoMock.Verify(r => r.ExcluirPorKrId("kr-2"), Times.Once);

            // Verifica exclusão dos KRs do objetivo
            _krRepoMock.Verify(r => r.ExcluirPorObjetivoId("obj-1"), Times.Once);

            // Verifica exclusão dos dados diretos do objetivo
            _comentarioRepoMock.Verify(r => r.ExcluirPorObjetivoId("obj-1"), Times.Once);
            _fatoRepoMock.Verify(r => r.ExcluirPorObjetivoId("obj-1"), Times.Once);
            _riscoRepoMock.Verify(r => r.ExcluirPorObjetivoId("obj-1"), Times.Once);

            // Verifica exclusão do objetivo
            _objetivoRepoMock.Verify(r => r.Excluir("obj-1"), Times.Once);
        }

        [Fact]
        public void Executar_ObjetivoNaoEncontrado_DeveRetornarErro()
        {
            // Arrange
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-999")).Returns((Objetivo?)null);

            // Act
            var resultado = _service.Executar("obj-999");

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Objetivo não encontrado.");
            _objetivoRepoMock.Verify(r => r.Excluir(It.IsAny<string>()), Times.Never);
        }
    }
}

