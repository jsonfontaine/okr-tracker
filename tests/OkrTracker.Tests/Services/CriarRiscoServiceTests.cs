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
    /// Testes unitários para CriarRiscoService.
    /// </summary>
    public class CriarRiscoServiceTests
    {
        private readonly Mock<IRiscoRepository> _riscoRepoMock;
        private readonly Mock<IObjetivoRepository> _objetivoRepoMock;
        private readonly Mock<IKeyResultRepository> _krRepoMock;
        private readonly Mock<ILogger<CriarRiscoService>> _loggerMock;
        private readonly CriarRiscoService _service;

        public CriarRiscoServiceTests()
        {
            _riscoRepoMock = new Mock<IRiscoRepository>();
            _objetivoRepoMock = new Mock<IObjetivoRepository>();
            _krRepoMock = new Mock<IKeyResultRepository>();
            _loggerMock = new Mock<ILogger<CriarRiscoService>>();
            _service = new CriarRiscoService(
                _riscoRepoMock.Object,
                _objetivoRepoMock.Object,
                _krRepoMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public void Executar_RiscoEmObjetivo_DeveRetornarSucesso()
        {
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(new Objetivo { Id = "obj-1" });
            var request = new CriarRiscoRequest
            {
                ObjetivoId = "obj-1",
                KrId = null,
                Descricao = "Dependência de outro time.",
                Impacto = "Alto"
            };

            var resultado = _service.Executar(request);

            resultado.Success.Should().BeTrue();
            resultado.Data!.Descricao.Should().Be("Dependência de outro time.");
            resultado.Data.Impacto.Should().Be("Alto");
            _riscoRepoMock.Verify(r => r.Inserir(It.IsAny<Risco>()), Times.Once);
        }

        [Fact]
        public void Executar_RiscoEmKR_DeveRetornarSucesso()
        {
            _krRepoMock.Setup(r => r.ObterPorId("kr-1")).Returns(new KeyResult { Id = "kr-1" });
            var request = new CriarRiscoRequest { KrId = "kr-1", Descricao = "Risco no KR." };
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeTrue();
        }

        [Fact]
        public void Executar_DescricaoVazia_DeveRetornarErro()
        {
            var request = new CriarRiscoRequest { ObjetivoId = "obj-1", Descricao = "" };
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Descrição do risco é obrigatória.");
        }

        [Fact]
        public void Executar_AmbosIds_DeveRetornarErro()
        {
            var request = new CriarRiscoRequest { ObjetivoId = "obj-1", KrId = "kr-1", Descricao = "Desc" };
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Informe exatamente um entre ObjetivoId e KrId.");
        }

        [Fact]
        public void Executar_NenhumId_DeveRetornarErro()
        {
            var request = new CriarRiscoRequest { Descricao = "Desc" };
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Informe exatamente um entre ObjetivoId e KrId.");
        }

        [Fact]
        public void Executar_ObjetivoNaoEncontrado_DeveRetornarErro()
        {
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-999")).Returns((Objetivo?)null);
            var request = new CriarRiscoRequest { ObjetivoId = "obj-999", Descricao = "Desc" };
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Objetivo não encontrado.");
        }

        [Fact]
        public void Executar_KRNaoEncontrado_DeveRetornarErro()
        {
            _krRepoMock.Setup(r => r.ObterPorId("kr-999")).Returns((KeyResult?)null);
            var request = new CriarRiscoRequest { KrId = "kr-999", Descricao = "Desc" };
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("KR não encontrado.");
        }

        [Fact]
        public void Executar_RiscoSemImpacto_DeveRetornarSucesso()
        {
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(new Objetivo { Id = "obj-1" });
            var request = new CriarRiscoRequest { ObjetivoId = "obj-1", Descricao = "Risco sem impacto." };
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeTrue();
            resultado.Data!.Impacto.Should().BeNull();
        }
    }
}
