using System.ComponentModel.DataAnnotations;

namespace Teste_Tecnico_INDT.Dominio;

public class Proposta
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string NomeCliente { get; private set; }

    [Required]
    [MaxLength(100)]
    public string Produto { get; private set; }

    public DateTime CriadaEmUtc { get; private set; } = DateTime.UtcNow;

    public StatusProposta Status { get; private set; } = StatusProposta.EmAnalise;

    public Proposta(string nomeCliente, string produto)
    {
        if (string.IsNullOrWhiteSpace(nomeCliente))
            throw new ArgumentException("Nome do cliente é obrigatório", nameof(nomeCliente));
        if (string.IsNullOrWhiteSpace(produto))
            throw new ArgumentException("Produto é obrigatório", nameof(produto));

        NomeCliente = nomeCliente.Trim();
        Produto = produto.Trim();
    }

    public void Aprovar()
    {
        if (Status != StatusProposta.EmAnalise)
            throw new InvalidOperationException("Apenas propostas em análise podem ser aprovadas");
        Status = StatusProposta.Aprovada;
    }

    public void Rejeitar()
    {
        if (Status != StatusProposta.EmAnalise)
            throw new InvalidOperationException("Apenas propostas em análise podem ser rejeitadas");
        Status = StatusProposta.Rejeitada;
    }
}


