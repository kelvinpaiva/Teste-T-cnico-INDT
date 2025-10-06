using Teste_Tecnico_INDT.Dominio;

namespace Teste_Tecnico_INDT.Aplicacao;

public class ServicoProposta : IServicoProposta
{
    private readonly IPropostaRepositorio _repositorio;

    public ServicoProposta(IPropostaRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<Proposta> CriarAsync(string nomeCliente, string produto, CancellationToken ct)
    {
        var proposta = new Proposta(nomeCliente, produto);
        await _repositorio.AdicionarAsync(proposta, ct);
        await _repositorio.SalvarAlteracoesAsync(ct);
        return proposta;
    }

    public Task<IReadOnlyList<Proposta>> ListarAsync(CancellationToken ct)
    {
        return _repositorio.ListarAsync(ct);
    }

    public async Task<Proposta?> AprovarAsync(Guid id, CancellationToken ct)
    {
        var proposta = await _repositorio.ObterPorIdAsync(id, ct);
        if (proposta == null) return null;
        proposta.Aprovar();
        await _repositorio.SalvarAlteracoesAsync(ct);
        return proposta;
    }

    public async Task<Proposta?> RejeitarAsync(Guid id, CancellationToken ct)
    {
        var proposta = await _repositorio.ObterPorIdAsync(id, ct);
        if (proposta == null) return null;
        proposta.Rejeitar();
        await _repositorio.SalvarAlteracoesAsync(ct);
        return proposta;
    }

    public Task<Proposta?> ObterAsync(Guid id, CancellationToken ct)
    {
        return _repositorio.ObterPorIdAsync(id, ct);
    }
}


