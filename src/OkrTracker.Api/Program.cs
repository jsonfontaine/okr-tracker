using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configuração dos controllers com serialização de enums como string
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger habilitado em todos os ambientes (aplicação local, single-user)
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
