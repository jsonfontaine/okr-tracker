using System.Text.Json.Serialization;
using OkrTracker.Application.Interfaces;
using OkrTracker.Application.Ports;
using OkrTracker.Application.Services;
using OkrTracker.Domain.Repositories;
using OkrTracker.Infrastructure.Persistence;
using OkrTracker.Infrastructure.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    var seqServerUrl = context.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341";

    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Seq(seqServerUrl);
});

// Configuração dos controllers com serialização de enums como string
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Infraestrutura ---
// DatabasePathProvider como Singleton (mantém o caminho configurado em memória durante a execução)
builder.Services.AddSingleton<IDatabasePathProvider, DatabasePathProvider>();
builder.Services.AddSingleton<IDatabaseValidator, LiteDbDatabaseValidator>();
builder.Services.AddSingleton<LiteDbConnectionFactory>();

// --- Repositórios ---
builder.Services.AddScoped<ICicloRepository, CicloRepository>();
builder.Services.AddScoped<IProjetoRepository, ProjetoRepository>();
builder.Services.AddScoped<IObjetivoRepository, ObjetivoRepository>();
builder.Services.AddScoped<IKeyResultRepository, KeyResultRepository>();
builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();
builder.Services.AddScoped<IFatoRelevanteRepository, FatoRelevanteRepository>();
builder.Services.AddScoped<IRiscoRepository, RiscoRepository>();

// --- Serviços de aplicação ---
builder.Services.AddScoped<IConfigurarBaseDeDadosService, ConfigurarBaseDeDadosService>();
builder.Services.AddScoped<ICriarCicloService, CriarCicloService>();
builder.Services.AddScoped<IListarCiclosService, ListarCiclosService>();
builder.Services.AddScoped<IAtualizarCicloService, AtualizarCicloService>();
builder.Services.AddScoped<IExcluirCicloService, ExcluirCicloService>();
builder.Services.AddScoped<ICriarProjetoService, CriarProjetoService>();
builder.Services.AddScoped<IListarProjetosService, ListarProjetosService>();
builder.Services.AddScoped<IAtualizarProjetoService, AtualizarProjetoService>();
builder.Services.AddScoped<IExcluirProjetoService, ExcluirProjetoService>();
builder.Services.AddScoped<ICriarObjetivoService, CriarObjetivoService>();
builder.Services.AddScoped<IAtualizarObjetivoService, AtualizarObjetivoService>();
builder.Services.AddScoped<IExcluirObjetivoService, ExcluirObjetivoService>();
builder.Services.AddScoped<IListarOKRsPorTimeECicloService, ListarOKRsPorTimeECicloService>();
builder.Services.AddScoped<ICriarKRService, CriarKRService>();
builder.Services.AddScoped<IAtualizarKRService, AtualizarKRService>();
builder.Services.AddScoped<IAtualizarProgressoKRService, AtualizarProgressoKRService>();
builder.Services.AddScoped<IExcluirKRService, ExcluirKRService>();
builder.Services.AddScoped<ICriarComentarioService, CriarComentarioService>();
builder.Services.AddScoped<ICriarFatoRelevanteService, CriarFatoRelevanteService>();
builder.Services.AddScoped<ICriarRiscoService, CriarRiscoService>();

var app = builder.Build();

// Swagger habilitado em todos os ambientes (aplicação local, single-user)
app.UseSwagger();
app.UseSwaggerUI();

// Servir arquivos estáticos do frontend (wwwroot)
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

// SPA fallback: qualquer rota que não seja /api/* serve o index.html do React
app.MapFallbackToFile("index.html");

app.Run();


