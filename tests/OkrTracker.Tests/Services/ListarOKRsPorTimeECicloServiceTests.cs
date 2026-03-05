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
    /// Testes unitários para ListarOKRsPorTimeECicloService.
    /// </summary>
    public class ListarOKRsPorTimeECicloServiceTests
    {
        private readonly Mock<IObjetivoRepository> _objetivoRepoMock;
        private readonly Mock<IKeyResultRepository> _krRepoMock;
        private readonly Mock<IComentarioRepository> _comentarioRepoMock;
        private readonly Mock<IFatoRelevanteRepository> _fatoRepoMock;
        private readonly Mock<IRiscoRepository> _riscoRepoMock;
        private readonly Mock<ILogger<ListarOKRsPorTimeECicloService>> _loggerMock;
        private readonly ListarOKRsPorTimeECicloService _service;

        public ListarOKRsPorTimeECicloServiceTests()
        {
            _objetivoRepoMock = new Mock<IObjetivoRepository>();
            _krRepoMock = new Mock<IKeyResultRepository>();
            _comentarioRepoMock = new Mock<IComentarioRepository>();
            _fatoRepoMock = new Mock<IFatoRelevanteRepository>();
            _riscoRepoMock = new Mock<IRiscoRepository>();
            _loggerMock = new Mock<ILogger<ListarOKRsPorTimeECicloService>>();
            _service = new ListarOKRsPorTimeECicloService(
                _objetivoRepoMock.Object,
                _krRepoMock.Object,
                _comentarioRepoMock.Object,
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
        public void Executar_SemObjetivos_DeveRetornarListaVazia()
        {
            _objetivoRepoMock.Setup(r => r.ObterPorCicloETime("ciclo-1", "time-1"))
                .Returns(new List<Objetivo>());

            var resultado = _service.Executar("ciclo-1", "time-1");
            resultado.Success.Should().BeTrue();
            resultado.Data.Should().BeEmpty();
        }

        [Fact]
        public void Executar_ComObjetivosEKRs_DeveRetornarDadosCompletos()
        {
            // Arrange
            var objetivo = new Objetivo
            {
                Id = "obj-1",
                Titulo = "Objetivo 1",
                Descricao = "Desc",
                CicloId = "ciclo-1",
                TimeId = "time-1",
                Prioridade = Prioridade.Alta,
                Progresso = 50,
                Status = Status.EmAndamentoAvancado,
                Farol = Farol.Verde
            };

            var kr = new KeyResult
            {
                Id = "kr-1",
                ObjetivoId = "obj-1",
                Titulo = "KR 1",
                Descricao = "Desc KR",
                Tipo = TipoKR.Quantitativo,
                Progresso = 50,
                Status = Status.EmAndamentoAvancado,
                Farol = Farol.Verde
            };

            _objetivoRepoMock.Setup(r => r.ObterPorCicloETime("ciclo-1", "time-1"))
                .Returns(new List<Objetivo> { objetivo });
            _krRepoMock.Setup(r => r.ObterPorObjetivoId("obj-1"))
                .Returns(new List<KeyResult> { kr });

            _comentarioRepoMock.Setup(r => r.ObterPorObjetivoId("obj-1")).Returns(new List<Comentario>
            {
                new() { Id = "com-1", Texto = "Check-in 1", ObjetivoId = "obj-1" }
            });
            _comentarioRepoMock.Setup(r => r.ObterPorKrId("kr-1")).Returns(new List<Comentario>());

            _fatoRepoMock.Setup(r => r.ObterPorObjetivoId("obj-1")).Returns(new List<FatoRelevante>());
            _fatoRepoMock.Setup(r => r.ObterPorKrId("kr-1")).Returns(new List<FatoRelevante>());

            _riscoRepoMock.Setup(r => r.ObterPorObjetivoId("obj-1")).Returns(new List<Risco>
            {
                new() { Id = "risco-1", Descricao = "Risco teste", Impacto = "Alto", ObjetivoId = "obj-1" }
            });
            _riscoRepoMock.Setup(r => r.ObterPorKrId("kr-1")).Returns(new List<Risco>());

            // Act
            var resultado = _service.Executar("ciclo-1", "time-1");

            // Assert
            resultado.Success.Should().BeTrue();
            var objetivos = resultado.Data!.ToList();
            objetivos.Should().HaveCount(1);
            objetivos[0].Titulo.Should().Be("Objetivo 1");
            objetivos[0].KeyResults.Should().HaveCount(1);
            objetivos[0].KeyResults[0].Titulo.Should().Be("KR 1");
            objetivos[0].Comentarios.Should().HaveCount(1);
            objetivos[0].Riscos.Should().HaveCount(1);
        }
    }
}
