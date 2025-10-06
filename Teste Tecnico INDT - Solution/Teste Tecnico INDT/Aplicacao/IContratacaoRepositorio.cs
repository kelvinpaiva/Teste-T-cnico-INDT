using Teste_Tecnico_INDT.Dominio;

namespace Teste_Tecnico_INDT.Aplicacao;

public interface IContratacaoRepositorio
{
    Task<Contratacao> AdicionarAsync(Contratacao contratacao, CancellationToken ct);
    Task<IReadOnlyList<Contratacao>> ListarAsync(CancellationToken ct);
}


