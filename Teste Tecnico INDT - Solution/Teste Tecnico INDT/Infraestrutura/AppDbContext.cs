using Microsoft.EntityFrameworkCore;
using Teste_Tecnico_INDT.Dominio;

namespace Teste_Tecnico_INDT.Infraestrutura;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Proposta> Propostas => Set<Proposta>();
    public DbSet<Contratacao> Contratacoes => Set<Contratacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var proposta = modelBuilder.Entity<Proposta>();
        proposta.HasKey(p => p.Id);
        proposta.Property(p => p.NomeCliente).IsRequired().HasMaxLength(200);
        proposta.Property(p => p.Produto).IsRequired().HasMaxLength(100);
        proposta.Property(p => p.CriadaEmUtc).IsRequired();
        proposta.Property(p => p.Status).IsRequired();
    }
}


