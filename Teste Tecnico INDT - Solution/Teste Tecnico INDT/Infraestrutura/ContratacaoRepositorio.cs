using Microsoft.EntityFrameworkCore;
using Teste_Tecnico_INDT.Aplicacao;
using Teste_Tecnico_INDT.Dominio;

namespace Teste_Tecnico_INDT.Infraestrutura;

public class ContratacaoRepositorio : IContratacaoRepositorio
{
    private readonly AppDbContext _dbContext;

    public ContratacaoRepositorio(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Contratacao> AdicionarAsync(Contratacao contratacao, CancellationToken ct)
    {
        await _dbContext.Contratacoes.AddAsync(contratacao, ct);
        await _dbContext.SaveChangesAsync(ct);
        return contratacao;
    }

    public async Task<IReadOnlyList<Contratacao>> ListarAsync(CancellationToken ct)
    {
        return await _dbContext.Contratacoes
            .OrderByDescending(c => c.DataContratacaoUtc)
            .ToListAsync(ct);
    }
}


