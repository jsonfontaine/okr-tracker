using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Application.Ports;

namespace OkrTracker.Application.Services
{
    /// <summary>
    /// Serviço responsável por configurar o caminho da base de dados LiteDB.
    /// Valida se o arquivo pode ser aberto antes de registrar.
    /// </summary>
    public class ConfigurarBaseDeDadosService : IConfigurarBaseDeDadosService
    {
        private readonly IDatabasePathProvider _pathProvider;
        private readonly IDatabaseValidator _validator;
        private readonly ILogger<ConfigurarBaseDeDadosService> _logger;

        public ConfigurarBaseDeDadosService(
            IDatabasePathProvider pathProvider,
            IDatabaseValidator validator,
            ILogger<ConfigurarBaseDeDadosService> logger)
        {
            _pathProvider = pathProvider;
            _validator = validator;
            _logger = logger;
        }

        /// <summary>
        /// Configura o caminho do arquivo .db e valida se o LiteDB consegue abrir.
        /// </summary>
        public ResultadoOperacao Configurar(string databasePath)
        {
            _logger.LogInformation("Tentando configurar base de dados no caminho: {Caminho}", databasePath);

            if (string.IsNullOrWhiteSpace(databasePath))
            {
                _logger.LogWarning("Caminho da base de dados não informado.");
                return ResultadoOperacao.Erro("O caminho da base de dados é obrigatório.");
            }

            try
            {
                var valido = _validator.Validar(databasePath);
                if (!valido)
                {
                    _logger.LogWarning("Não foi possível abrir a base de dados no caminho: {Caminho}", databasePath);
                    return ResultadoOperacao.Erro("Não foi possível abrir a base de dados.");
                }

                _pathProvider.DefinirCaminho(databasePath);
                _logger.LogInformation("Base de dados configurada com sucesso no caminho: {Caminho}", databasePath);
                return ResultadoOperacao.Sucesso();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao configurar base de dados no caminho: {Caminho}", databasePath);
                return ResultadoOperacao.Erro("Não foi possível abrir a base de dados.");
            }
        }
    }
}
