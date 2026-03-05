using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OkrTracker.Application.Ports;
using OkrTracker.Application.Services;

namespace OkrTracker.Tests.Services
{
    /// <summary>
    /// Testes unitários para ConfigurarBaseDeDadosService.
    /// </summary>
    public class ConfigurarBaseDeDadosServiceTests
    {
        private readonly Mock<IDatabasePathProvider> _pathProviderMock;
        private readonly Mock<IDatabaseValidator> _validatorMock;
        private readonly Mock<ILogger<ConfigurarBaseDeDadosService>> _loggerMock;
        private readonly ConfigurarBaseDeDadosService _service;

        public ConfigurarBaseDeDadosServiceTests()
        {
            _pathProviderMock = new Mock<IDatabasePathProvider>();
            _validatorMock = new Mock<IDatabaseValidator>();
            _loggerMock = new Mock<ILogger<ConfigurarBaseDeDadosService>>();
            _service = new ConfigurarBaseDeDadosService(_pathProviderMock.Object, _validatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Configurar_CaminhoValido_DeveRetornarSucesso()
        {
            // Arrange
            var caminho = @"C:\dados\okr.db";
            _validatorMock.Setup(v => v.Validar(caminho)).Returns(true);

            // Act
            var resultado = _service.Configurar(caminho);

            // Assert
            resultado.Success.Should().BeTrue();
            _pathProviderMock.Verify(p => p.DefinirCaminho(caminho), Times.Once);
        }

        [Fact]
        public void Configurar_CaminhoVazio_DeveRetornarErro()
        {
            // Act
            var resultado = _service.Configurar("");

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("O caminho da base de dados é obrigatório.");
        }

        [Fact]
        public void Configurar_CaminhoNulo_DeveRetornarErro()
        {
            // Act
            var resultado = _service.Configurar(null!);

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("O caminho da base de dados é obrigatório.");
        }

        [Fact]
        public void Configurar_CaminhoInvalido_DeveRetornarErro()
        {
            // Arrange
            var caminho = @"C:\invalido\nao_existe.db";
            _validatorMock.Setup(v => v.Validar(caminho)).Returns(false);

            // Act
            var resultado = _service.Configurar(caminho);

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Não foi possível abrir a base de dados.");
            _pathProviderMock.Verify(p => p.DefinirCaminho(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Configurar_ValidadorLancaExcecao_DeveRetornarErro()
        {
            // Arrange
            var caminho = @"C:\dados\okr.db";
            _validatorMock.Setup(v => v.Validar(caminho)).Throws(new Exception("Erro inesperado"));

            // Act
            var resultado = _service.Configurar(caminho);

            // Assert
            resultado.Success.Should().BeFalse();
            resultado.Message.Should().Be("Não foi possível abrir a base de dados.");
        }
    }
}
