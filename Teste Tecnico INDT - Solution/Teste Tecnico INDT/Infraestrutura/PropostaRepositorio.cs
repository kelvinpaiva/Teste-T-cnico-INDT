using Microsoft.EntityFrameworkCore;
using Teste_Tecnico_INDT.Aplicacao;
using Teste_Tecnico_INDT.Dominio;

namespace Teste_Tecnico_INDT.Infraestrutura;

public class PropostaRepositorio : IPropostaRepositorio
{
    private readonly AppDbContext _dbContext;

    public PropostaRepositorio(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Proposta> AdicionarAsync(Proposta proposta, CancellationToken ct)
    {
        await _dbContext.Propostas.AddAsync(proposta, ct);
        return proposta;
    }

    public Task<Proposta?> ObterPorIdAsync(Guid id, CancellationToken ct)
    {
        return _dbContext.Propostas.FirstOrDefaultAsync(p => p.Id == id, ct)!;
    }

    public async Task<IReadOnlyList<Proposta>> ListarAsync(CancellationToken ct)
    {
        return await _dbContext.Propostas
            .OrderByDescending(p => p.CriadaEmUtc)
            .ToListAsync(ct);
    }

    public Task SalvarAlteracoesAsync(CancellationToken ct)
    {
        return _dbContext.SaveChangesAsync(ct);
    }
}


