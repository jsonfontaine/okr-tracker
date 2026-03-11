using LiteDB;
using System;
using System.Linq;

var dbPath = args.Length > 0 ? args[0] : "../../database/jason-okr-tracker.db";
using var db = new LiteDatabase(dbPath);
var collections = db.GetCollectionNames().ToList();
Console.WriteLine("Coleções no banco:");
foreach (var colName in collections)
{
    Console.WriteLine($"- {colName}");
    var col = db.GetCollection(colName);
    var docs = col.FindAll().ToList();
    Console.WriteLine($"  Documentos: {docs.Count}");
    foreach (var doc in docs)
    {
        Console.WriteLine($"    {doc}");
    }
}
