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
            var kr = new KeyResult { Id = "kr-1", ObjetivoId = "obj-1", Tipo = TipoKR.Quantitativo, Progresso = 0, Status = Status.NaoIniciado };
            _krRepoMock.Setup(r => r.ObterPorId("kr-1")).Returns(kr);
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(new Objetivo { Id = "obj-1", Status = Status.NaoIniciado });
            _krRepoMock.Setup(r => r.ObterPorObjetivoId("obj-1")).Returns(new List<KeyResult> { kr });

            var resultado = _service.Executar("kr-1", new AtualizarProgressoRequest { Progresso = 45 });

            resultado.Success.Should().BeTrue();
            resultado.Data!.Progresso.Should().Be(45);
            resultado.Data.Status.Should().Be("NaoIniciado");
            _objetivoRepoMock.Verify(r => r.Atualizar(It.Is<Objetivo>(o => o.Progresso == 45)), Times.Once);
        }

        [Fact]
        public void Executar_Progresso100_MarcaObjetivoComoConcluido()
        {
            var kr = new KeyResult { Id = "kr-1", ObjetivoId = "obj-1", Tipo = TipoKR.Quantitativo, Progresso = 50, Status = Status.EmAndamento };
            _krRepoMock.Setup(r => r.ObterPorId("kr-1")).Returns(kr);
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(new Objetivo { Id = "obj-1", Status = Status.EmAndamento });
            _krRepoMock.Setup(r => r.ObterPorObjetivoId("obj-1")).Returns(new List<KeyResult> { new() { Progresso = 100 } });

            var resultado = _service.Executar("kr-1", new AtualizarProgressoRequest { Progresso = 100 });

            resultado.Success.Should().BeTrue();
            _objetivoRepoMock.Verify(r => r.Atualizar(It.Is<Objetivo>(o => o.Progresso == 100 && o.Status == Status.Concluido)), Times.Once);
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
        public void Executar_DeveRecalcularProgressoDoObjetivo()
        {
            var kr1 = new KeyResult { Id = "kr-1", ObjetivoId = "obj-1", Tipo = TipoKR.Quantitativo, Progresso = 0, Status = Status.NaoIniciado };
            var objetivo = new Objetivo { Id = "obj-1", Status = Status.NaoIniciado };

            _krRepoMock.Setup(r => r.ObterPorId("kr-1")).Returns(kr1);
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(objetivo);

            _krRepoMock.Setup(r => r.ObterPorObjetivoId("obj-1")).Returns(new List<KeyResult>
            {
                new() { Id = "kr-1", Progresso = 60 },
                new() { Id = "kr-2", Progresso = 80 }
            });

            var resultado = _service.Executar("kr-1", new AtualizarProgressoRequest { Progresso = 60 });

            resultado.Success.Should().BeTrue();
            _objetivoRepoMock.Verify(r => r.Atualizar(It.Is<Objetivo>(o => o.Progresso == 70)), Times.Once);
        }
    }
}
