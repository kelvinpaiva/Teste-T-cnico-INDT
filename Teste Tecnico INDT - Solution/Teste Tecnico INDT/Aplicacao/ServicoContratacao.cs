using Teste_Tecnico_INDT.Dominio;
using Teste_Tecnico_INDT.Infraestrutura;

namespace Teste_Tecnico_INDT.Aplicacao;

public class ServicoContratacao : IServicoContratacao
{
    private readonly IPropostaRepositorio _propostas;
    private readonly IContratacaoRepositorio _contratacoes;

    public ServicoContratacao(IPropostaRepositorio propostas, IContratacaoRepositorio contratacoes)
    {
        _propostas = propostas;
        _contratacoes = contratacoes;
    }

    public async Task<Contratacao?> ContratarAsync(Guid propostaId, CancellationToken ct)
    {
        var proposta = await _propostas.ObterPorIdAsync(propostaId, ct);
        if (proposta == null) return null;
        if (proposta.Status != StatusProposta.Aprovada)
            throw new InvalidOperationException("Somente propostas aprovadas podem ser contratadas");

        var contratacao = new Contratacao(proposta.Id);
        await _contratacoes.AdicionarAsync(contratacao, ct);
        return contratacao;
    }

    public Task<IReadOnlyList<Contratacao>> ListarAsync(CancellationToken ct)
    {
        return _contratacoes.ListarAsync(ct);
    }
}


