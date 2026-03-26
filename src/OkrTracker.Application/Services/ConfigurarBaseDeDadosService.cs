using Microsoft.Extensions.Logging;
using OkrTracker.Application.DTOs;
using OkrTracker.Application.Interfaces;
using OkrTracker.Application.Ports;
using System.Runtime.InteropServices;

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
                var caminhoNormalizado = NormalizarCaminho(databasePath.Trim());

                if (!File.Exists(caminhoNormalizado))
                {
                    _logger.LogWarning("Arquivo .db não encontrado no caminho: {Caminho}", caminhoNormalizado);
                    return ResultadoOperacao.Erro("Arquivo .db não encontrado. No container, use caminhos como /data/arquivo.db.");
                }

                var valido = _validator.Validar(caminhoNormalizado);
                if (!valido)
                {
                    _logger.LogWarning("Não foi possível abrir a base de dados no caminho: {Caminho}", caminhoNormalizado);
                    return ResultadoOperacao.Erro("Não foi possível abrir a base de dados.");
                }

                _pathProvider.DefinirCaminho(caminhoNormalizado);
                _logger.LogInformation("Base de dados configurada com sucesso no caminho: {Caminho}", caminhoNormalizado);
                return ResultadoOperacao.Sucesso();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao configurar base de dados no caminho: {Caminho}", databasePath);
                return ResultadoOperacao.Erro("Não foi possível abrir a base de dados.");
            }
        }

        private string NormalizarCaminho(string databasePath)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return databasePath;
            }

            // Em Linux (container), tenta mapear caminho Windows para o volume /data pelo nome do arquivo.
            if (databasePath.Contains("\\") && databasePath.Contains(":"))
            {
                var fileName = Path.GetFileName(databasePath.Replace('\\', '/'));
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    var mountedPath = Path.Combine("/data", fileName);
                    if (File.Exists(mountedPath))
                    {
                        _logger.LogInformation("Caminho Windows detectado em ambiente Linux. Usando caminho mapeado: {Caminho}", mountedPath);
                        return mountedPath;
                    }
                }
            }

            return databasePath;
        }
    }
}
