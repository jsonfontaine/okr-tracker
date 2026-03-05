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
    /// Testes unitários para AtualizarKRService.
    /// </summary>
    public class AtualizarKRServiceTests
    {
        private readonly Mock<IKeyResultRepository> _krRepoMock;
        private readonly Mock<ILogger<AtualizarKRService>> _loggerMock;
        private readonly AtualizarKRService _service;

        public AtualizarKRServiceTests()
        {
            _krRepoMock = new Mock<IKeyResultRepository>();
            _loggerMock = new Mock<ILogger<AtualizarKRService>>();
            _service = new AtualizarKRService(_krRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Executar_DadosValidos_DeveRetornarSucesso()
        {
            // Arrange
            var kr = new KeyResult
            {
                Id = "kr-1",
                ObjetivoId = "obj-1",
                Titulo = "Antigo",
                Descricao = "Antiga",
                Tipo = TipoKR.Quantitativo,
                Progresso = 30,
                DataCriacao = DateTime.UtcNow
            };
            _krRepoMock.Setup(r => r.ObterPorId("kr-1")).Returns(kr);

            var request = new AtualizarKeyResultRequest
            {
                Titulo = "Novo Título",
                Descricao = "Nova Descrição",
                Tipo = "Qualitativo",
                Farol = "Amarelo"
            };

            // Act
            var resultado = _service.Executar("kr-1", request);

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data!.Titulo.Should().Be("Novo Título");
            resultado.Data.Tipo.Should().Be("Qualitativo");
            resultado.Data.Farol.Should().Be("Amarelo");
            _krRepoMock.Verify(r => r.Atualizar(It.IsAny<KeyResult>()), Times.Once);
        }

        [Fact]
        public void Executar_TituloVazio_DeveRetornarErro()
        {
            var request = new AtualizarKeyResultRequest { Titulo = "", Descricao = "Desc", Tipo = "Quantitativo" };
            var resultado = _service.Executar("kr-1", request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Título do KR é obrigatório.");
        }

        [Fact]
        public void Executar_DescricaoVazia_DeveRetornarErro()
        {
            var request = new AtualizarKeyResultRequest { Titulo = "Título", Descricao = "", Tipo = "Quantitativo" };
            var resultado = _service.Executar("kr-1", request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Descrição do KR é obrigatória.");
        }

        [Fact]
        public void Executar_TipoInvalido_DeveRetornarErro()
        {
            var request = new AtualizarKeyResultRequest { Titulo = "Título", Descricao = "Desc", Tipo = "Invalido" };
            var resultado = _service.Executar("kr-1", request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Selecione um tipo válido para o KR.");
        }

        [Fact]
        public void Executar_KRNaoEncontrado_DeveRetornarErro()
        {
            _krRepoMock.Setup(r => r.ObterPorId("kr-999")).Returns((KeyResult?)null);
            var request = new AtualizarKeyResultRequest { Titulo = "T", Descricao = "D", Tipo = "Quantitativo" };
            var resultado = _service.Executar("kr-999", request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("KR não encontrado.");
        }
    }
}
