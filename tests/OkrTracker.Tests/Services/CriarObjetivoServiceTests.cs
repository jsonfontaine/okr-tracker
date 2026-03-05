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
    /// Testes unitários para CriarObjetivoService.
    /// </summary>
    public class CriarObjetivoServiceTests
    {
        private readonly Mock<IObjetivoRepository> _objetivoRepoMock;
        private readonly Mock<ICicloRepository> _cicloRepoMock;
        private readonly Mock<ITimeRepository> _timeRepoMock;
        private readonly Mock<ILogger<CriarObjetivoService>> _loggerMock;
        private readonly CriarObjetivoService _service;

        public CriarObjetivoServiceTests()
        {
            _objetivoRepoMock = new Mock<IObjetivoRepository>();
            _cicloRepoMock = new Mock<ICicloRepository>();
            _timeRepoMock = new Mock<ITimeRepository>();
            _loggerMock = new Mock<ILogger<CriarObjetivoService>>();
            _service = new CriarObjetivoService(
                _objetivoRepoMock.Object,
                _cicloRepoMock.Object,
                _timeRepoMock.Object,
                _loggerMock.Object);
        }

        private CriarObjetivoRequest CriarRequestValido()
        {
            return new CriarObjetivoRequest
            {
                Titulo = "Melhorar previsibilidade",
                Descricao = "Criar processos e métricas",
                CicloId = "ciclo-1",
                TimeId = "time-1",
                Prioridade = "Alta",
                Farol = "Verde"
            };
        }

        [Fact]
        public void Executar_DadosValidos_DeveRetornarSucesso()
        {
            // Arrange
            _cicloRepoMock.Setup(r => r.ObterPorId("ciclo-1")).Returns(new Ciclo { Id = "ciclo-1", Nome = "2026-Q1" });
            _timeRepoMock.Setup(r => r.ObterPorId("time-1")).Returns(new Time { Id = "time-1", Nome = "Bridge" });

            // Act
            var resultado = _service.Executar(CriarRequestValido());

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data!.Titulo.Should().Be("Melhorar previsibilidade");
            resultado.Data.Progresso.Should().Be(0);
            resultado.Data.Status.Should().Be("NaoIniciado");
            resultado.Data.Prioridade.Should().Be("Alta");
            _objetivoRepoMock.Verify(r => r.Inserir(It.IsAny<Objetivo>()), Times.Once);
        }

        [Fact]
        public void Executar_TituloVazio_DeveRetornarErro()
        {
            // Arrange
            var request = CriarRequestValido();
            request.Titulo = "";

            // Act
            var resultado = _service.Executar(request);

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Título do objetivo é obrigatório.");
        }

        [Fact]
        public void Executar_DescricaoVazia_DeveRetornarErro()
        {
            // Arrange
            var request = CriarRequestValido();
            request.Descricao = "";

            // Act
            var resultado = _service.Executar(request);

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Descrição do objetivo é obrigatória.");
        }

        [Fact]
        public void Executar_CicloIdVazio_DeveRetornarErro()
        {
            // Arrange
            var request = CriarRequestValido();
            request.CicloId = "";

            // Act
            var resultado = _service.Executar(request);

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("O ciclo é obrigatório.");
        }

        [Fact]
        public void Executar_TimeIdVazio_DeveRetornarErro()
        {
            // Arrange
            var request = CriarRequestValido();
            request.TimeId = "";

            // Act
            var resultado = _service.Executar(request);

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("O time é obrigatório.");
        }

        [Fact]
        public void Executar_CicloNaoEncontrado_DeveRetornarErro()
        {
            // Arrange
            _cicloRepoMock.Setup(r => r.ObterPorId("ciclo-1")).Returns((Ciclo?)null);

            // Act
            var resultado = _service.Executar(CriarRequestValido());

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Ciclo não encontrado.");
        }

        [Fact]
        public void Executar_TimeNaoEncontrado_DeveRetornarErro()
        {
            // Arrange
            _cicloRepoMock.Setup(r => r.ObterPorId("ciclo-1")).Returns(new Ciclo { Id = "ciclo-1" });
            _timeRepoMock.Setup(r => r.ObterPorId("time-1")).Returns((Time?)null);

            // Act
            var resultado = _service.Executar(CriarRequestValido());

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Time não encontrado.");
        }

        [Fact]
        public void Executar_PrioridadeInvalida_DeveUsarMedia()
        {
            // Arrange
            var request = CriarRequestValido();
            request.Prioridade = "Invalida";
            _cicloRepoMock.Setup(r => r.ObterPorId("ciclo-1")).Returns(new Ciclo { Id = "ciclo-1" });
            _timeRepoMock.Setup(r => r.ObterPorId("time-1")).Returns(new Time { Id = "time-1" });

            // Act
            var resultado = _service.Executar(request);

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data!.Prioridade.Should().Be("Media");
        }
    }
}
