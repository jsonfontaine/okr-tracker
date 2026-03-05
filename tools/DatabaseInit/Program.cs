using LiteDB;

// ============================================================================
// OKR Tracker — Utilitário de inicialização do banco de dados LiteDB
// 
// Uso:
//   dotnet run                              → cria na pasta database/ padrão
//   dotnet run -- "C:\caminho\meu-banco.db" → cria no caminho personalizado
//
// Este utilitário pode ser executado quantas vezes for necessário.
// Se o arquivo já existir, apenas garante que os índices estão criados.
// ============================================================================

var defaultPath = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "database", "jason-okr-tracker.db"));

var dbPath = args.Length > 0 ? args[0] : defaultPath;

Console.WriteLine("=== OKR Tracker — Database Init ===");
Console.WriteLine();

// Garantir que o diretório existe
var directory = Path.GetDirectoryName(dbPath);
if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
{
    Directory.CreateDirectory(directory);
    Console.WriteLine($"Diretório criado: {directory}");
}

Console.WriteLine($"Caminho do banco: {dbPath}");

var jaExistia = File.Exists(dbPath);

using (var db = new LiteDatabase(dbPath))
{
    // Inicializa as coleções com os índices definidos na arquitetura
    var ciclos = db.GetCollection<BsonDocument>("ciclos");
    ciclos.EnsureIndex("nome", true);

    var times = db.GetCollection<BsonDocument>("times");
    times.EnsureIndex("nome", true);

    var objetivos = db.GetCollection<BsonDocument>("objetivos");
    objetivos.EnsureIndex("cicloId");
    objetivos.EnsureIndex("timeId");

    var krs = db.GetCollection<BsonDocument>("krs");
    krs.EnsureIndex("objetivoId");

    var comentarios = db.GetCollection<BsonDocument>("comentarios");
    comentarios.EnsureIndex("objetivoId");
    comentarios.EnsureIndex("krId");

    var fatosRelevantes = db.GetCollection<BsonDocument>("fatosRelevantes");
    fatosRelevantes.EnsureIndex("objetivoId");
    fatosRelevantes.EnsureIndex("krId");

    var riscos = db.GetCollection<BsonDocument>("riscos");
    riscos.EnsureIndex("objetivoId");
    riscos.EnsureIndex("krId");

    Console.WriteLine();
    Console.WriteLine(jaExistia
        ? "Banco já existia — índices verificados/atualizados."
        : "Banco criado com sucesso!");

    Console.WriteLine($"Coleções: {string.Join(", ", db.GetCollectionNames())}");
}

Console.WriteLine();
Console.WriteLine("Concluído.");
