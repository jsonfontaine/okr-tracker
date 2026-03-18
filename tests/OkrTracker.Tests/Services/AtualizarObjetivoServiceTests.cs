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
    /// Testes unitários para AtualizarObjetivoService.
    /// </summary>
    public class AtualizarObjetivoServiceTests
    {
        private readonly Mock<IObjetivoRepository> _objetivoRepoMock;
        private readonly Mock<ICicloRepository> _cicloRepoMock;
        private readonly Mock<IProjetoRepository> _projetoRepoMock;
        private readonly Mock<IComentarioRepository> _comentarioRepoMock;
        private readonly Mock<ILogger<AtualizarObjetivoService>> _loggerMock;
        private readonly AtualizarObjetivoService _service;

        public AtualizarObjetivoServiceTests()
        {
            _objetivoRepoMock = new Mock<IObjetivoRepository>();
            _cicloRepoMock = new Mock<ICicloRepository>();
            _projetoRepoMock = new Mock<IProjetoRepository>();
            _comentarioRepoMock = new Mock<IComentarioRepository>();
            _loggerMock = new Mock<ILogger<AtualizarObjetivoService>>();
            _service = new AtualizarObjetivoService(
                _objetivoRepoMock.Object,
                _cicloRepoMock.Object,
                _projetoRepoMock.Object,
                _comentarioRepoMock.Object,
                _loggerMock.Object);
        }

        private AtualizarObjetivoRequest CriarRequestValido()
        {
            return new AtualizarObjetivoRequest
            {
                Titulo = "Título atualizado",
                Descricao = "Descrição atualizada",
                CicloId = "ciclo-1",
                ProjetoId = "projeto-1",
                Prioridade = "Alta",
                Farol = "Amarelo",
                Valor = "Maior visibilidade para a liderança"
            };
        }

        [Fact]
        public void Executar_DadosValidos_DeveRetornarSucesso()
        {
            // Arrange
            var objetivo = new Objetivo { Id = "obj-1", Titulo = "Antigo", Descricao = "Antiga", CicloId = "ciclo-1", ProjetoId = "projeto-1", DataCriacao = DateTime.UtcNow };
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(objetivo);
            _cicloRepoMock.Setup(r => r.ObterPorId("ciclo-1")).Returns(new Ciclo { Id = "ciclo-1" });
            _projetoRepoMock.Setup(r => r.ObterPorId("projeto-1")).Returns(new Projeto { Id = "projeto-1" });

            // Act
            var resultado = _service.Executar("obj-1", CriarRequestValido());

            // Assert
            resultado.Success.Should().BeTrue();
            resultado.Data!.Titulo.Should().Be("Título atualizado");
            resultado.Data.Farol.Should().Be("Amarelo");
            _objetivoRepoMock.Verify(r => r.Atualizar(It.IsAny<Objetivo>()), Times.Once);
        }

        [Fact]
        public void Executar_TituloVazio_DeveRetornarErro()
        {
            var request = CriarRequestValido();
            request.Titulo = "";
            var resultado = _service.Executar("obj-1", request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Título do objetivo é obrigatório.");
        }

        [Fact]
        public void Executar_DescricaoVazia_DeveRetornarErro()
        {
            var request = CriarRequestValido();
            request.Descricao = "";
            var resultado = _service.Executar("obj-1", request);
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Descrição do objetivo é obrigatória.");
        }

        [Fact]
        public void Executar_ObjetivoNaoEncontrado_DeveRetornarErro()
        {
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-999")).Returns((Objetivo?)null);
            var resultado = _service.Executar("obj-999", CriarRequestValido());
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Objetivo não encontrado.");
        }

        [Fact]
        public void Executar_CicloNaoEncontrado_DeveRetornarErro()
        {
            var objetivo = new Objetivo { Id = "obj-1", CicloId = "ciclo-1", ProjetoId = "projeto-1" };
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(objetivo);
            _cicloRepoMock.Setup(r => r.ObterPorId("ciclo-1")).Returns((Ciclo?)null);

            var resultado = _service.Executar("obj-1", CriarRequestValido());
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Ciclo não encontrado.");
        }

        [Fact]
        public void Executar_ProjetoNaoEncontrado_DeveRetornarErro()
        {
            var objetivo = new Objetivo { Id = "obj-1", CicloId = "ciclo-1", ProjetoId = "projeto-1" };
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(objetivo);
            _cicloRepoMock.Setup(r => r.ObterPorId("ciclo-1")).Returns(new Ciclo { Id = "ciclo-1" });
            _projetoRepoMock.Setup(r => r.ObterPorId("projeto-1")).Returns((Projeto?)null);

            var resultado = _service.Executar("obj-1", CriarRequestValido());
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Projeto não encontrado.");
        }

        [Fact]
        public void Executar_AlteracaoDeFarol_DeveCriarComentarioAutomatico()
        {
            var objetivo = new Objetivo { Id = "obj-1", Titulo = "T", Descricao = "D", CicloId = "ciclo-1", ProjetoId = "projeto-1", Farol = Farol.Verde, Valor = "V" };
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(objetivo);
            _cicloRepoMock.Setup(r => r.ObterPorId("ciclo-1")).Returns(new Ciclo { Id = "ciclo-1" });
            _projetoRepoMock.Setup(r => r.ObterPorId("projeto-1")).Returns(new Projeto { Id = "projeto-1" });

            var request = CriarRequestValido();
            request.Farol = "Vermelho";

            var resultado = _service.Executar("obj-1", request);

            resultado.Success.Should().BeTrue();
            _comentarioRepoMock.Verify(r => r.Inserir(It.Is<Comentario>(c =>
                c.ObjetivoId == "obj-1" &&
                c.Texto.Contains("Farol alterado"))), Times.Once);
        }

        [Fact]
        public void Executar_AlteracaoDeCiclo_DeveCriarComentarioAutomatico()
        {
            var objetivo = new Objetivo
            {
                Id = "obj-1",
                Titulo = "T",
                Descricao = "D",
                CicloId = "ciclo-1",
                ProjetoId = "projeto-1",
                Farol = Farol.Amarelo,
                Status = Status.NaoIniciado,
                Prioridade = Prioridade.Alta,
                Valor = "V"
            };

            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(objetivo);
            _cicloRepoMock.Setup(r => r.ObterPorId("ciclo-2")).Returns(new Ciclo { Id = "ciclo-2" });
            _projetoRepoMock.Setup(r => r.ObterPorId("projeto-1")).Returns(new Projeto { Id = "projeto-1" });

            var request = CriarRequestValido();
            request.CicloId = "ciclo-2";
            request.Farol = "Amarelo";
            request.Prioridade = "Alta";
            request.Status = "NaoIniciado";

            var resultado = _service.Executar("obj-1", request);

            resultado.Success.Should().BeTrue();
            _comentarioRepoMock.Verify(r => r.Inserir(It.Is<Comentario>(c =>
                c.ObjetivoId == "obj-1" &&
                c.Texto.Contains("Ciclo alterado"))), Times.Once);
        }

        [Fact]
        public void Executar_SemAlteracaoDeFarolStatusPrioridade_NaoDeveCriarComentario()
        {
            var objetivo = new Objetivo { Id = "obj-1", Titulo = "T", Descricao = "D", CicloId = "ciclo-1", ProjetoId = "projeto-1", Farol = Farol.Amarelo, Status = Status.NaoIniciado, Prioridade = Prioridade.Alta, Valor = "V" };
            _objetivoRepoMock.Setup(r => r.ObterPorId("obj-1")).Returns(objetivo);
            _cicloRepoMock.Setup(r => r.ObterPorId("ciclo-1")).Returns(new Ciclo { Id = "ciclo-1" });
            _projetoRepoMock.Setup(r => r.ObterPorId("projeto-1")).Returns(new Projeto { Id = "projeto-1" });

            var resultado = _service.Executar("obj-1", CriarRequestValido());

            resultado.Success.Should().BeTrue();
            _comentarioRepoMock.Verify(r => r.Inserir(It.IsAny<Comentario>()), Times.Never);
        }
    }
}
