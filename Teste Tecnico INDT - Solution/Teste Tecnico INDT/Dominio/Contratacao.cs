namespace Teste_Tecnico_INDT.Dominio;

public class Contratacao
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PropostaId { get; private set; }
    public DateTime DataContratacaoUtc { get; private set; } = DateTime.UtcNow;

    public Contratacao(Guid propostaId)
    {
        if (propostaId == Guid.Empty) throw new ArgumentException("PropostaId inválido", nameof(propostaId));
        PropostaId = propostaId;
    }
}


