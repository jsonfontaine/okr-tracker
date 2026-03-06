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
    /// Testes unitários para ExportarResumoExecutivoService.
    /// </summary>
    public class ExportarResumoExecutivoServiceTests
    {
        private readonly Mock<IObjetivoRepository> _objetivoRepoMock;
        private readonly Mock<IKeyResultRepository> _krRepoMock;
        private readonly Mock<IFatoRelevanteRepository> _fatoRepoMock;
        private readonly Mock<IRiscoRepository> _riscoRepoMock;
        private readonly Mock<ICicloRepository> _cicloRepoMock;
        private readonly Mock<ITimeRepository> _timeRepoMock;
        private readonly Mock<ILogger<ExportarResumoExecutivoService>> _loggerMock;
        private readonly ExportarResumoExecutivoService _service;

        public ExportarResumoExecutivoServiceTests()
        {
            _objetivoRepoMock = new Mock<IObjetivoRepository>();
            _krRepoMock = new Mock<IKeyResultRepository>();
            _fatoRepoMock = new Mock<IFatoRelevanteRepository>();
            _riscoRepoMock = new Mock<IRiscoRepository>();
            _cicloRepoMock = new Mock<ICicloRepository>();
            _timeRepoMock = new Mock<ITimeRepository>();
            _loggerMock = new Mock<ILogger<ExportarResumoExecutivoService>>();
            _service = new ExportarResumoExecutivoService(
                _objetivoRepoMock.Object,
                _krRepoMock.Object,
                _fatoRepoMock.Object,
                _riscoRepoMock.Object,
                _cicloRepoMock.Object,
                _timeRepoMock.Object,
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
        public void Executar_SemOKRs_DeveRetornarResumoVazio()
        {
            _cicloRepoMock.Setup(r => r.ObterPorId("ciclo-1")).Returns(new Ciclo { Id = "ciclo-1", Nome = "2026-Q1" });
            _timeRepoMock.Setup(r => r.ObterPorId("time-1")).Returns(new Time { Id = "time-1", Nome = "Bridge" });
            _objetivoRepoMock.Setup(r => r.ObterPorCicloETime("ciclo-1", "time-1"))
                .Returns(new List<Objetivo>());

            var resultado = _service.Executar("ciclo-1", "time-1");

            resultado.Success.Should().BeTrue();
            resultado.Data.Should().NotBeNullOrEmpty();
            resultado.Data.Should().Contain("RESUMO EXECUTIVO");
            resultado.Data.Should().Contain("Bridge");
            resultado.Data.Should().Contain("2026-Q1");
            resultado.Data.Should().Contain("Não há OKRs cadastrados");
        }

        [Fact]
        public void Executar_ComOKRs_DeveRetornarResumoComDados()
        {
            // Arrange
            _cicloRepoMock.Setup(r => r.ObterPorId("ciclo-1")).Returns(new Ciclo { Id = "ciclo-1", Nome = "2026-Q1" });
            _timeRepoMock.Setup(r => r.ObterPorId("time-1")).Returns(new Time { Id = "time-1", Nome = "Bridge" });

            var objetivo = new Objetivo
            {
                Id = "obj-1",
                Titulo = "Objetivo Export",
                Descricao = "Desc",
                CicloId = "ciclo-1",
                TimeId = "time-1",
                Prioridade = Prioridade.Alta,
                Progresso = 60,
                Status = Status.EmAndamento,
                Farol = Farol.Verde,
                Valor = "Valor para o negócio"
            };

            var kr = new KeyResult
            {
                Id = "kr-1",
                ObjetivoId = "obj-1",
                Titulo = "KR Export",
                Descricao = "Desc KR",
                Tipo = TipoKR.Quantitativo,
                Progresso = 60,
                Status = Status.EmAndamento,
                Farol = Farol.Verde
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
            resultado.Data.Should().NotBeNullOrEmpty();
            resultado.Data.Should().Contain("RESUMO EXECUTIVO");
            resultado.Data.Should().Contain("Objetivo Export");
            resultado.Data.Should().Contain("60%");
            resultado.Data.Should().Contain("KR Export");
            resultado.Data.Should().Contain("Valor para o negócio");
            resultado.Data.Should().Contain("Fato export");
            resultado.Data.Should().Contain("Risco export");
            resultado.Data.Should().Contain("Impacto: Alto");
            resultado.Data.Should().Contain("Bridge");
            resultado.Data.Should().Contain("2026-Q1");
        }
    }
}
