using LiteDB;
using System;
using System.Text.RegularExpressions;

namespace OkrTracker.Tools.DatabaseInit
{
    /// <summary>
    /// Migração para preencher DataInicio e DataFim dos ciclos existentes
    /// baseado no padrão do nome (ex: 2026-Q1, 2026-S1).
    /// Fallback: usa DataCriacao se não conseguir fazer parse do nome.
    /// </summary>
    public class LiteDbMigrationCicloDates
    {
        public static void Executar(ILiteCollection<BsonDocument> cicloColl)
        {
            Console.WriteLine("🔄 Iniciando migração de datas de ciclos...");

            var ciclos = cicloColl.FindAll().ToList();
            int atualizados = 0;
            int comErro = 0;

            foreach (var ciclo in ciclos)
            {
                // Se já tem datas, pula
                if (ciclo.ContainsKey("dataInicio") && ciclo["dataInicio"].AsDateTime != DateTime.MinValue)
                {
                    continue;
                }

                try
                {
                    var nome = ciclo["nome"].AsString;
                    var (dataInicio, dataFim) = ExtrairDatasDoNome(nome);

                    if (dataInicio == DateTime.MinValue)
                    {
                        // Fallback: usar DataCriacao
                        var dataCriacao = ciclo["dataCriacao"].AsDateTime;
                        dataInicio = dataCriacao;
                        dataFim = dataCriacao.AddMonths(3); // Assumir ciclo de 3 meses
                        Console.WriteLine($"⚠️  Ciclo '{nome}' sem padrão reconhecido. Usando DataCriacao como fallback.");
                    }

                    ciclo["dataInicio"] = dataInicio;
                    ciclo["dataFim"] = dataFim;

                    cicloColl.Update(ciclo);
                    atualizados++;

                    Console.WriteLine($"✅ Ciclo '{nome}': {dataInicio:yyyy-MM-dd} a {dataFim:yyyy-MM-dd}");
                }
                catch (Exception ex)
                {
                    comErro++;
                    Console.WriteLine($"❌ Erro ao processar ciclo: {ex.Message}");
                }
            }

            Console.WriteLine($"\n✨ Migração concluída! {atualizados} ciclos atualizados, {comErro} com erro.");
        }

        private static (DateTime dataInicio, DateTime dataFim) ExtrairDatasDoNome(string nome)
        {
            // Padrões suportados: 2026-Q1, 2026-S1, 2026-Q2, etc.
            var match = Regex.Match(nome, @"(\d{4})[-_]?([QS])(\d)");

            if (!match.Success)
                return (DateTime.MinValue, DateTime.MinValue);

            int ano = int.Parse(match.Groups[1].Value);
            string tipo = match.Groups[2].Value;
            int numero = int.Parse(match.Groups[3].Value);

            if (tipo == "Q")
            {
                // Quarter: Q1 = Jan-Mar, Q2 = Abr-Jun, Q3 = Jul-Set, Q4 = Out-Dez
                if (numero < 1 || numero > 4)
                    return (DateTime.MinValue, DateTime.MinValue);

                int mesInicio = (numero - 1) * 3 + 1;
                var dataInicio = new DateTime(ano, mesInicio, 1);
                var dataFim = dataInicio.AddMonths(3).AddDays(-1);

                return (dataInicio, dataFim);
            }
            else if (tipo == "S")
            {
                // Semester: S1 = Jan-Jun, S2 = Jul-Dez
                if (numero < 1 || numero > 2)
                    return (DateTime.MinValue, DateTime.MinValue);

                int mesInicio = (numero - 1) * 6 + 1;
                var dataInicio = new DateTime(ano, mesInicio, 1);
                var dataFim = dataInicio.AddMonths(6).AddDays(-1);

                return (dataInicio, dataFim);
            }

            return (DateTime.MinValue, DateTime.MinValue);
        }
    }
}

