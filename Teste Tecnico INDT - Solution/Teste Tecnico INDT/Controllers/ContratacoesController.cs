using Microsoft.AspNetCore.Mvc;
using Teste_Tecnico_INDT.Api;
using Teste_Tecnico_INDT.Aplicacao;

namespace Teste_Tecnico_INDT.Controllers;

[ApiController]
[Route("api/contratacoes")]
public class ContratacoesController : ControllerBase
{
    private readonly IServicoContratacao _servico;

    public ContratacoesController(IServicoContratacao servico)
    {
        _servico = servico;
    }

    [HttpPost]
    public async Task<ActionResult<ContratacaoResponse>> Criar([FromBody] CriarContratacaoRequest request, CancellationToken ct)
    {
        var contratacao = await _servico.ContratarAsync(request.PropostaId, ct);
        if (contratacao == null) return NotFound();
        return CreatedAtAction(nameof(Listar), new { }, ContratacaoResponse.From(contratacao));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContratacaoResponse>>> Listar(CancellationToken ct)
    {
        var lista = await _servico.ListarAsync(ct);
        return Ok(lista.Select(ContratacaoResponse.From));
    }
}


