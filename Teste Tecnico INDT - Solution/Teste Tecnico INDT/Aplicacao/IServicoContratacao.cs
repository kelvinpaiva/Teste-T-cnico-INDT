using Teste_Tecnico_INDT.Dominio;

namespace Teste_Tecnico_INDT.Aplicacao;

public interface IServicoContratacao
{
    Task<Contratacao?> ContratarAsync(Guid propostaId, CancellationToken ct);
    Task<IReadOnlyList<Contratacao>> ListarAsync(CancellationToken ct);
}


