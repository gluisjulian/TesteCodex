using Microsoft.AspNetCore.Mvc;
using Perfumes.Api.DTOs;
using Perfumes.Api.Services;
namespace Perfumes.Api.Controllers;
[ApiController, Route("api/fornecedores")]
public sealed class FornecedoresController(FornecedorService service) : ControllerBase
{
    [HttpGet]
    public Task<PaginaDto<FornecedorResponseDto>> Listar([FromQuery] string? busca, [FromQuery] bool? ativo,
        CancellationToken ct, [FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 20) =>
        service.Listar(busca, ativo, pagina, tamanhoPagina, ct);
    [HttpGet("{id:long}")]
    public Task<FornecedorResponseDto> Obter(long id, CancellationToken ct) => service.Obter(id, ct);
    [HttpPost]
    public async Task<ActionResult<FornecedorResponseDto>> Criar(FornecedorSalvarDto dto, CancellationToken ct)
    {
        var result = await service.Salvar(null, dto, ct);
        return CreatedAtAction(nameof(Obter), new { id = result.Id }, result);
    }
    [HttpPut("{id:long}")]
    public Task<FornecedorResponseDto> Atualizar(long id, FornecedorSalvarDto dto, CancellationToken ct) => service.Salvar(id, dto, ct);
    [HttpPatch("{id:long}/status")]
    public async Task<IActionResult> Status(long id, StatusDto dto, CancellationToken ct)
    {
        await service.AlterarStatus(id, dto.Ativo, ct);
        return NoContent();
    }
}

