using LiteDB;
using System;
using System.Linq;

// Migração LiteDB: times -> projetos
var dbPath = args.Length > 0 ? args[0] : "../../database/jason-okr-tracker.db";
using var db = new LiteDatabase(dbPath);
var times = db.GetCollection("times").FindAll().ToList();
var projetos = db.GetCollection("projetos");
int count = 0;
foreach (var doc in times)
{
    // Evita duplicidade: só insere se não existe
    var id = doc["_id"];
    if (projetos.FindById(id) == null)
    {
        projetos.Insert(doc);
        count++;
    }
}
Console.WriteLine($"Migrados {count} projetos para a coleção 'projetos'.");
