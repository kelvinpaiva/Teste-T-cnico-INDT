using Microsoft.EntityFrameworkCore;
using Teste_Tecnico_INDT.Aplicacao;
using Teste_Tecnico_INDT.Infraestrutura;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var conn = builder.Configuration.GetConnectionString("Default") ?? "Data Source=propostas.db";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(conn));

builder.Services.AddScoped<IPropostaRepositorio, PropostaRepositorio>();
builder.Services.AddScoped<IServicoProposta, ServicoProposta>();
builder.Services.AddScoped<IContratacaoRepositorio, ContratacaoRepositorio>();
builder.Services.AddScoped<IServicoContratacao, ServicoContratacao>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// aplica migrações automaticamente na inicialização
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapControllers();

app.Run();
