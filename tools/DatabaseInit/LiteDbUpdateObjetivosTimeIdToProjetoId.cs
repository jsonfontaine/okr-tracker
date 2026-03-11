using LiteDB;
using System;
using System.Linq;

// Script para atualizar todos os objetivos: TimeId -> ProjetoId
var dbPath = args.Length > 0 ? args[0] : "../../database/jason-okr-tracker.db";
using var db = new LiteDatabase(dbPath);
var objetivos = db.GetCollection("objetivos");
int updated = 0;
foreach (var doc in objetivos.FindAll().ToList())
{
    if (doc.ContainsKey("TimeId"))
    {
        doc["ProjetoId"] = doc["TimeId"];
        doc.Remove("TimeId");
        objetivos.Update(doc);
        updated++;
    }
}
Console.WriteLine($"Atualizados {updated} objetivos: TimeId -> ProjetoId.");

