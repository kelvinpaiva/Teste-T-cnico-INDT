using Teste_Tecnico_INDT.Dominio;

namespace Teste_Tecnico_INDT.Aplicacao;

public interface IPropostaRepositorio
{
    Task<Proposta> AdicionarAsync(Proposta proposta, CancellationToken ct);
    Task<Proposta?> ObterPorIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Proposta>> ListarAsync(CancellationToken ct);
    Task SalvarAlteracoesAsync(CancellationToken ct);
}


