using Microsoft.AspNetCore.Mvc;
using Perfumes.Api.DTOs;
using Perfumes.Api.Services;
namespace Perfumes.Api.Controllers;
[ApiController, Route("api/fornecedores/{fornecedorId:long}/produtos")]
public sealed class FornecedorProdutosController(FornecedorProdutoService service) : ControllerBase
{
    [HttpGet]
    public Task<PaginaDto<FornecedorProdutoResponseDto>> Listar(long fornecedorId, [FromQuery] bool? ativo,
        CancellationToken ct, [FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 20) =>
        service.Listar(fornecedorId, ativo, pagina, tamanhoPagina, ct);
    [HttpGet("{id:long}")]
    public Task<FornecedorProdutoResponseDto> Obter(long fornecedorId, long id, CancellationToken ct) => service.Obter(fornecedorId, id, ct);
    [HttpPost]
    public async Task<ActionResult<FornecedorProdutoResponseDto>> Criar(long fornecedorId, FornecedorProdutoCreateDto dto, CancellationToken ct)
    {
        var result = await service.Criar(fornecedorId, dto, ct);
        return CreatedAtAction(nameof(Obter), new { fornecedorId, id = result.Id }, result);
    }
    [HttpPut("{id:long}")]
    public Task<FornecedorProdutoResponseDto> Atualizar(long fornecedorId, long id, FornecedorProdutoSalvarDto dto, CancellationToken ct) =>
        service.Atualizar(fornecedorId, id, dto, ct);
    [HttpPatch("{id:long}/status")]
    public async Task<IActionResult> Status(long fornecedorId, long id, StatusDto dto, CancellationToken ct)
    {
        await service.AlterarStatus(fornecedorId, id, dto.Ativo, ct);
        return NoContent();
    }
}

