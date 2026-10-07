using Microsoft.AspNetCore.Mvc;
using Perfumes.Api.DTOs;
using Perfumes.Api.Services;
namespace Perfumes.Api.Controllers;
[ApiController, Route("api/fornecedor-produtos/{id:long}")]
public sealed class PrecosController(PrecoService service) : ControllerBase
{
    [HttpGet("precos")]
    public Task<PaginaDto<FornecedorProdutoPrecoResponseDto>> Historico(long id, CancellationToken ct,
        [FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 20) => service.Historico(id, pagina, tamanhoPagina, ct);
    [HttpGet("preco-atual")]
    public Task<FornecedorProdutoPrecoResponseDto> Atual(long id, CancellationToken ct) => service.Atual(id, ct);
    [HttpPost("precos")]
    public async Task<ActionResult<FornecedorProdutoPrecoResponseDto>> Alterar(long id, FornecedorProdutoPrecoCreateDto dto, CancellationToken ct)
    {
        var result = await service.Alterar(id, dto, ct);
        return CreatedAtAction(nameof(Historico), new { id }, result);
    }
}

