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
    /// Testes unitários para ExportarAdaptiveCardService.
    /// </summary>
    public class ExportarAdaptiveCardServiceTests
    {
        private readonly Mock<IObjetivoRepository> _objetivoRepoMock;
        private readonly Mock<IKeyResultRepository> _krRepoMock;
        private readonly Mock<IFatoRelevanteRepository> _fatoRepoMock;
        private readonly Mock<IRiscoRepository> _riscoRepoMock;
        private readonly Mock<ILogger<ExportarAdaptiveCardService>> _loggerMock;
        private readonly ExportarAdaptiveCardService _service;

        public ExportarAdaptiveCardServiceTests()
        {
            _objetivoRepoMock = new Mock<IObjetivoRepository>();
            _krRepoMock = new Mock<IKeyResultRepository>();
            _fatoRepoMock = new Mock<IFatoRelevanteRepository>();
            _riscoRepoMock = new Mock<IRiscoRepository>();
            _loggerMock = new Mock<ILogger<ExportarAdaptiveCardService>>();
            _service = new ExportarAdaptiveCardService(
                _objetivoRepoMock.Object,
                _krRepoMock.Object,
                _fatoRepoMock.Object,
                _riscoRepoMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public void Executar_CicloIdVazio_DeveRetornarErro()
        {
            var resultado = _service.Executar("", "time-1");
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("O cicloId é obrigatório.");
        }

        [Fact]
        public void Executar_TimeIdVazio_DeveRetornarErro()
        {
            var resultado = _service.Executar("ciclo-1", "");
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("O timeId é obrigatório.");
        }

        [Fact]
        public void Executar_SemOKRs_DeveRetornarCardVazio()
        {
            _objetivoRepoMock.Setup(r => r.ObterPorCicloETime("ciclo-1", "time-1"))
                .Returns(new List<Objetivo>());

            var resultado = _service.Executar("ciclo-1", "time-1");

            resultado.Success.Should().BeTrue();
            resultado.Data.Should().NotBeNull();

            // Verifica se tem a propriedade type = AdaptiveCard
            var tipo = resultado.Data!.GetType().GetProperty("type")?.GetValue(resultado.Data)?.ToString();
            tipo.Should().Be("AdaptiveCard");

            var versao = resultado.Data.GetType().GetProperty("version")?.GetValue(resultado.Data)?.ToString();
            versao.Should().Be("1.5");
        }

        [Fact]
        public void Executar_ComOKRs_DeveRetornarCardComDados()
        {
            // Arrange
            var objetivo = new Objetivo
            {
                Id = "obj-1",
                Titulo = "Objetivo Export",
                Descricao = "Desc",
                CicloId = "ciclo-1",
                TimeId = "time-1",
                Prioridade = Prioridade.Alta,
                Progresso = 60,
                Status = Status.EmAndamentoAvancado,
                Farol = Farol.Verde
            };

            var kr = new KeyResult
            {
                Id = "kr-1",
                ObjetivoId = "obj-1",
                Titulo = "KR Export",
                Descricao = "Desc KR",
                Tipo = TipoKR.Quantitativo,
                Progresso = 60,
                Status = Status.EmAndamentoAvancado
            };

            _objetivoRepoMock.Setup(r => r.ObterPorCicloETime("ciclo-1", "time-1"))
                .Returns(new List<Objetivo> { objetivo });
            _krRepoMock.Setup(r => r.ObterPorObjetivoId("obj-1"))
                .Returns(new List<KeyResult> { kr });
            _fatoRepoMock.Setup(r => r.ObterPorObjetivoId("obj-1"))
                .Returns(new List<FatoRelevante>
                {
                    new() { Id = "f-1", Texto = "Fato export", ObjetivoId = "obj-1" }
                });
            _riscoRepoMock.Setup(r => r.ObterPorObjetivoId("obj-1"))
                .Returns(new List<Risco>
                {
                    new() { Id = "r-1", Descricao = "Risco export", Impacto = "Alto", ObjetivoId = "obj-1" }
                });

            // Act
            var resultado = _service.Executar("ciclo-1", "time-1");

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data.Should().NotBeNull();

            // Verifica estrutura do AdaptiveCard
            var body = resultado.Data!.GetType().GetProperty("body")?.GetValue(resultado.Data) as List<object>;
            body.Should().NotBeNull();
            body!.Count.Should().BeGreaterThan(2); // Título + separador + objetivo + ...
        }
    }
}
