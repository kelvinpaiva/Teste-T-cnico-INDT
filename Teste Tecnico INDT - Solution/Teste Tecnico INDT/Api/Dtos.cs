using System.ComponentModel.DataAnnotations;
using Teste_Tecnico_INDT.Dominio;

namespace Teste_Tecnico_INDT.Api;

public record CriarPropostaRequest
{
    [Required]
    [MaxLength(200)]
    public string NomeCliente { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Produto { get; init; } = string.Empty;
}

public record PropostaResponse
{
    public Guid Id { get; init; }
    public string NomeCliente { get; init; } = string.Empty;
    public string Produto { get; init; } = string.Empty;
    public DateTime CriadaEmUtc { get; init; }
    public StatusProposta Status { get; init; }

    public static PropostaResponse From(Proposta p) => new()
    {
        Id = p.Id,
        NomeCliente = p.NomeCliente,
        Produto = p.Produto,
        CriadaEmUtc = p.CriadaEmUtc,
        Status = p.Status
    };
}


