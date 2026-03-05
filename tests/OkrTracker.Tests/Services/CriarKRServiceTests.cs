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
    /// Testes unitários para CriarKRService.
    /// </summary>
    public class CriarKRServiceTests
    {
        private readonly Mock<IKeyResultRepository> _krRepoMock;
        private readonly Mock<IObjetivoRepository> _objetivoRepoMock;
        private readonly Mock<ILogger<CriarKRService>> _loggerMock;
        private readonly CriarKRService _service;

        public CriarKRServiceTests()
        {
            _krRepoMock = new Mock<IKeyResultRepository>();
            _objetivoRepoMock = new Mock<IObjetivoRepository>();
            _loggerMock = new Mock<ILogger<CriarKRService>>();
            _service = new CriarKRService(_krRepoMock.Object, _objetivoRepoMock.Object, _loggerMock.Object);
        }

        private CriarKeyResultRequest CriarRequestValido()
        {
            return new CriarKeyResultRequest
            {
                ObjetivoId = "obj-1",
                Titulo = "Atingir 80% de cumprimento",
                Descricao = "Monitorar entregas",
                Tipo = "Quantitativo",
                Progresso = 0,
                Farol = "Verde"
            };
        }

        [Fact]
        public void Executar_DadosValidos_DeveRetornarSucesso()
        {
            // Arrange
            var objetivo = new Objetivo { Id = "obj-1", CicloId = "c1", TimeId = "t1" };
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(objetivo);
            _krRepoMock.Setup(r => r.ObterPorObjetivoId("obj-1")).Returns(new List<KeyResult>());

            // Act
            var resultado = _service.Executar(CriarRequestValido());

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data!.Titulo.Should().Be("Atingir 80% de cumprimento");
            resultado.Data.Tipo.Should().Be("Quantitativo");
            resultado.Data.Progresso.Should().Be(0);
            _krRepoMock.Verify(r => r.Inserir(It.IsAny<KeyResult>()), Times.Once);
        }

        [Fact]
        public void Executar_TituloVazio_DeveRetornarErro()
        {
            var request = CriarRequestValido();
            request.Titulo = "";
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Título do KR é obrigatório.");
        }

        [Fact]
        public void Executar_DescricaoVazia_DeveRetornarErro()
        {
            var request = CriarRequestValido();
            request.Descricao = "";
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Descrição do KR é obrigatória.");
        }

        [Fact]
        public void Executar_ObjetivoIdVazio_DeveRetornarErro()
        {
            var request = CriarRequestValido();
            request.ObjetivoId = "";
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("O objetivo é obrigatório.");
        }

        [Fact]
        public void Executar_TipoInvalido_DeveRetornarErro()
        {
            var request = CriarRequestValido();
            request.Tipo = "Invalido";
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Selecione um tipo válido para o KR.");
        }

        [Fact]
        public void Executar_ObjetivoNaoEncontrado_DeveRetornarErro()
        {
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns((Objetivo?)null);
            var resultado = _service.Executar(CriarRequestValido());
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Objetivo não encontrado.");
        }

        [Fact]
        public void Executar_ProgressoNegativo_DeveRetornarErro()
        {
            var request = CriarRequestValido();
            request.Progresso = -10;
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(new Objetivo { Id = "obj-1" });
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Valor de progresso inválido.");
        }

        [Fact]
        public void Executar_ProgressoAcima100_DeveRetornarErro()
        {
            var request = CriarRequestValido();
            request.Progresso = 150;
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(new Objetivo { Id = "obj-1" });
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Valor de progresso inválido.");
        }

        [Fact]
        public void Executar_TipoRequisito_ProgressoDiferenteDe0Ou100_DeveRetornarErro()
        {
            var request = CriarRequestValido();
            request.Tipo = "Requisito";
            request.Progresso = 50;
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(new Objetivo { Id = "obj-1" });
            var resultado = _service.Executar(request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Para KR do tipo Requisito, o progresso só pode ser 0 ou 100.");
        }

        [Fact]
        public void Executar_TipoRequisito_Progresso0_DeveRetornarSucesso()
        {
            var request = CriarRequestValido();
            request.Tipo = "Requisito";
            request.Progresso = 0;
            var objetivo = new Objetivo { Id = "obj-1" };
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(objetivo);
            _krRepoMock.Setup(r => r.ObterPorObjetivoId("obj-1")).Returns(new List<KeyResult>());

            var resultado = _service.Executar(request);
            resultado.Success.Should().BeTrue();
            resultado.Data!.Tipo.Should().Be("Requisito");
        }

        [Fact]
        public void Executar_TipoRequisito_Progresso100_DeveRetornarSucesso()
        {
            var request = CriarRequestValido();
            request.Tipo = "Requisito";
            request.Progresso = 100;
            var objetivo = new Objetivo { Id = "obj-1" };
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(objetivo);
            _krRepoMock.Setup(r => r.ObterPorObjetivoId("obj-1")).Returns(new List<KeyResult>());

            var resultado = _service.Executar(request);
            resultado.Success.Should().BeTrue();
        }

        [Fact]
        public void Executar_DeveRecalcularProgressoDoObjetivo()
        {
            // Arrange
            var objetivo = new Objetivo { Id = "obj-1" };
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(objetivo);

            // Simula que após inserir haverá 2 KRs: um com 50% e o recém-criado com 0%
            var krsExistentes = new List<KeyResult>
            {
                new() { Id = "kr-1", ObjetivoId = "obj-1", Progresso = 50 }
            };

            // O setup retorna os KRs APÓS a inserção (incluindo o novo)
            _krRepoMock.Setup(r => r.ObterPorObjetivoId("obj-1"))
                .Returns(() =>
                {
                    // Simula a lista com o KR existente + o novo (progresso 0)
                    return new List<KeyResult>
                    {
                        new() { Id = "kr-1", ObjetivoId = "obj-1", Progresso = 50 },
                        new() { Id = "kr-new", ObjetivoId = "obj-1", Progresso = 0 }
                    };
                });

            // Act
            var resultado = _service.Executar(CriarRequestValido());

            // Assert
            resultado.Success.Should().BeTrue();
            _objetivoRepoMock.Verify(r => r.Atualizar(It.Is<Objetivo>(o => o.Progresso == 25)), Times.Once);
        }
    }
}
