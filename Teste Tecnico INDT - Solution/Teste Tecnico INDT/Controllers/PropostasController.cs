using Microsoft.AspNetCore.Mvc;
using Teste_Tecnico_INDT.Api;
using Teste_Tecnico_INDT.Aplicacao;

namespace Teste_Tecnico_INDT.Controllers;

[ApiController]
[Route("api/propostas")]
public class PropostasController : ControllerBase
{
    private readonly IServicoProposta _servico;

    public PropostasController(IServicoProposta servico)
    {
        _servico = servico;
    }

    [HttpPost]
    public async Task<ActionResult<PropostaResponse>> Criar([FromBody] CriarPropostaRequest request, CancellationToken ct)
    {
        var proposta = await _servico.CriarAsync(request.NomeCliente, request.Produto, ct);
        return CreatedAtAction(nameof(Obter), new { id = proposta.Id }, PropostaResponse.From(proposta));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PropostaResponse>>> Listar(CancellationToken ct)
    {
        var propostas = await _servico.ListarAsync(ct);
        return Ok(propostas.Select(PropostaResponse.From));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PropostaResponse>> Obter([FromRoute] Guid id, CancellationToken ct)
    {
        var proposta = await _servico.ObterAsync(id, ct);
        if (proposta == null) return NotFound();
        return Ok(PropostaResponse.From(proposta));
    }

    [HttpPost("{id:guid}/aprovar")]
    public async Task<ActionResult<PropostaResponse>> Aprovar([FromRoute] Guid id, CancellationToken ct)
    {
        var proposta = await _servico.AprovarAsync(id, ct);
        if (proposta == null) return NotFound();
        return Ok(PropostaResponse.From(proposta));
    }

    [HttpPost("{id:guid}/rejeitar")]
    public async Task<ActionResult<PropostaResponse>> Rejeitar([FromRoute] Guid id, CancellationToken ct)
    {
        var proposta = await _servico.RejeitarAsync(id, ct);
        if (proposta == null) return NotFound();
        return Ok(PropostaResponse.From(proposta));
    }
}


