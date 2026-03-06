using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Services;
using OkrTracker.Domain.Entities;
using OkrTracker.Domain.Enums;
using OkrTracker.Domain.Repositories;

namespace OkrTracker.Tests.Services
{
    /// <summary>
    /// Testes unitários para AtualizarProgressoKRService.
    /// </summary>
    public class AtualizarProgressoKRServiceTests
    {
        private readonly Mock<IKeyResultRepository> _krRepoMock;
        private readonly Mock<IObjetivoRepository> _objetivoRepoMock;
        private readonly Mock<ILogger<AtualizarProgressoKRService>> _loggerMock;
        private readonly AtualizarProgressoKRService _service;

        public AtualizarProgressoKRServiceTests()
        {
            _krRepoMock = new Mock<IKeyResultRepository>();
            _objetivoRepoMock = new Mock<IObjetivoRepository>();
            _loggerMock = new Mock<ILogger<AtualizarProgressoKRService>>();
            _service = new AtualizarProgressoKRService(_krRepoMock.Object, _objetivoRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Executar_ProgressoValido_DeveRetornarSucesso()
        {
            // Arrange
            var kr = new KeyResult { Id = "kr-1", ObjetivoId = "obj-1", Tipo = TipoKR.Quantitativo, Progresso = 0, Status = Status.NaoIniciado };
            _krRepoMock.Setup(r => r.ObterPorId("kr-1")).Returns(kr);

            // Act
            var resultado = _service.Executar("kr-1", new AtualizarProgressoRequest { Progresso = 45 });

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data!.Progresso.Should().Be(45);
            resultado.Data.Status.Should().Be("NaoIniciado");
        }

        [Fact]
        public void Executar_Progresso100_NaoAlteraStatusAutomaticamente()
        {
            // Arrange
            var kr = new KeyResult { Id = "kr-1", ObjetivoId = "obj-1", Tipo = TipoKR.Quantitativo, Progresso = 50, Status = Status.EmAndamento };
            _krRepoMock.Setup(r => r.ObterPorId("kr-1")).Returns(kr);

            // Act
            var resultado = _service.Executar("kr-1", new AtualizarProgressoRequest { Progresso = 100 });

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data!.Progresso.Should().Be(100);
            resultado.Data.Status.Should().Be("EmAndamento");
        }

        [Fact]
        public void Executar_KRNaoEncontrado_DeveRetornarErro()
        {
            _krRepoMock.Setup(r => r.ObterPorId("kr-999")).Returns((KeyResult?)null);
            var resultado = _service.Executar("kr-999", new AtualizarProgressoRequest { Progresso = 50 });
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("KR não encontrado.");
        }

        [Fact]
        public void Executar_ProgressoNegativo_DeveRetornarErro()
        {
            var kr = new KeyResult { Id = "kr-1", ObjetivoId = "obj-1", Tipo = TipoKR.Quantitativo };
            _krRepoMock.Setup(r => r.ObterPorId("kr-1")).Returns(kr);

            var resultado = _service.Executar("kr-1", new AtualizarProgressoRequest { Progresso = -5 });
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Valor de progresso inválido.");
        }

        [Fact]
        public void Executar_ProgressoAcima100_DeveRetornarErro()
        {
            var kr = new KeyResult { Id = "kr-1", ObjetivoId = "obj-1", Tipo = TipoKR.Quantitativo };
            _krRepoMock.Setup(r => r.ObterPorId("kr-1")).Returns(kr);

            var resultado = _service.Executar("kr-1", new AtualizarProgressoRequest { Progresso = 120 });
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Valor de progresso inválido.");
        }

        [Fact]
        public void Executar_TipoRequisito_Progresso50_DeveRetornarErro()
        {
            var kr = new KeyResult { Id = "kr-1", ObjetivoId = "obj-1", Tipo = TipoKR.Requisito };
            _krRepoMock.Setup(r => r.ObterPorId("kr-1")).Returns(kr);

            var resultado = _service.Executar("kr-1", new AtualizarProgressoRequest { Progresso = 50 });
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Para KR do tipo Requisito, o progresso só pode ser 0 ou 100.");
        }

        [Fact]
        public void Executar_TipoRequisito_Progresso100_DeveRetornarSucesso()
        {
            var kr = new KeyResult { Id = "kr-1", ObjetivoId = "obj-1", Tipo = TipoKR.Requisito, Progresso = 0, Status = Status.NaoIniciado };
            _krRepoMock.Setup(r => r.ObterPorId("kr-1")).Returns(kr);

            var resultado = _service.Executar("kr-1", new AtualizarProgressoRequest { Progresso = 100 });
            resultado.Success.Should().BeTrue();
            resultado.Data!.Progresso.Should().Be(100);
        }

        [Fact]
        public void Executar_NaoDeveRecalcularProgressoDoObjetivo()
        {
            var kr = new KeyResult { Id = "kr-1", ObjetivoId = "obj-1", Tipo = TipoKR.Quantitativo, Progresso = 0, Status = Status.NaoIniciado };
            _krRepoMock.Setup(r => r.ObterPorId("kr-1")).Returns(kr);

            var resultado = _service.Executar("kr-1", new AtualizarProgressoRequest { Progresso = 60 });

            resultado.Success.Should().BeTrue();
            _objetivoRepoMock.Verify(r => r.Atualizar(It.IsAny<Objetivo>()), Times.Never);
        }
    }
}
