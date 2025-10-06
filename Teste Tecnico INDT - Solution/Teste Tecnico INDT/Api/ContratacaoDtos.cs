using System.ComponentModel.DataAnnotations;
using Teste_Tecnico_INDT.Dominio;

namespace Teste_Tecnico_INDT.Api;

public record CriarContratacaoRequest
{
    [Required]
    public Guid PropostaId { get; init; }
}

public record ContratacaoResponse
{
    public Guid Id { get; init; }
    public Guid PropostaId { get; init; }
    public DateTime DataContratacaoUtc { get; init; }

    public static ContratacaoResponse From(Contratacao c) => new()
    {
        Id = c.Id,
        PropostaId = c.PropostaId,
        DataContratacaoUtc = c.DataContratacaoUtc
    };
}


