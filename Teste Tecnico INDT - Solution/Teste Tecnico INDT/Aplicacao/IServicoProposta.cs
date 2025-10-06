using Teste_Tecnico_INDT.Dominio;

namespace Teste_Tecnico_INDT.Aplicacao;

public interface IServicoProposta
{
    Task<Proposta> CriarAsync(string nomeCliente, string produto, CancellationToken ct);
    Task<IReadOnlyList<Proposta>> ListarAsync(CancellationToken ct);
    Task<Proposta?> ObterAsync(Guid id, CancellationToken ct);
    Task<Proposta?> AprovarAsync(Guid id, CancellationToken ct);
    Task<Proposta?> RejeitarAsync(Guid id, CancellationToken ct);
}


