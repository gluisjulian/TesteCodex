using Microsoft.EntityFrameworkCore;
using Perfumes.Api.Data;
using Perfumes.Api.DTOs;
using Perfumes.Api.Entities;
namespace Perfumes.Api.Services;
public sealed class InsumoService(ApplicationDbContext db)
{
    public async Task<PaginaDto<InsumoResponseDto>> Listar(string? busca, bool? ativo, int pagina, int tamanho, CancellationToken ct)
    {
        FornecedorService.ValidarPaginacao(pagina, tamanho);
        var q = db.Insumos.AsNoTracking();
        if (ativo.HasValue) q = q.Where(x => x.Ativo == ativo);
        if (!string.IsNullOrWhiteSpace(busca)) q = q.Where(x => x.Nome.Contains(busca.Trim()));
        var total = await q.CountAsync(ct);
        var itens = await q.OrderBy(x => x.Nome).ThenBy(x => x.Id).Skip((pagina-1)*tamanho).Take(tamanho).ToListAsync(ct);
        return new(itens.Select(x => x.ParaDto()).ToList(), total, pagina, tamanho);
    }
    private async Task<Insumo> Entidade(long id, CancellationToken ct) =>
        await db.Insumos.SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw new RegraException(404, "Insumo não encontrado.");
    public async Task<InsumoResponseDto> Obter(long id, CancellationToken ct) => (await Entidade(id, ct)).ParaDto();
    public async Task<InsumoResponseDto> Salvar(long? id, InsumoSalvarDto dto, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, ct);
        if (!Enum.IsDefined(dto.TipoInsumo) || dto.UnidadeMedida is not (UnidadeMedida.ML or UnidadeMedida.G or UnidadeMedida.UN))
            throw new RegraException(400, "Tipo inválido ou unidade base diferente de ML, G e UN.");
        if (dto.Densidade.HasValue) UnidadeService.ValidarDecimal(dto.Densidade.Value, 6, 999999999999.999999m);
        var x = id.HasValue ? await Entidade(id.Value, ct) : new Insumo();
        if (id.HasValue && x.UnidadeMedida != dto.UnidadeMedida && await db.FornecedorProdutos.AnyAsync(p => p.InsumoId == id, ct))
            throw new RegraException(409, "Unidade base não pode mudar quando existem ofertas.");
        x.Nome = dto.Nome.Trim();
        if (x.Nome.Length == 0) throw new RegraException(400, "Nome obrigatório.");
        x.Descricao = FornecedorService.Limpar(dto.Descricao); x.TipoInsumo = dto.TipoInsumo;
        x.UnidadeMedida = dto.UnidadeMedida; x.Densidade = dto.Densidade;
        x.Observacao = FornecedorService.Limpar(dto.Observacao); x.Ativo = dto.Ativo;
        if (!id.HasValue) db.Insumos.Add(x);
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return x.ParaDto();
    }
    public async Task AlterarStatus(long id, bool ativo, CancellationToken ct)
    {
        (await Entidade(id, ct)).Ativo = ativo;
        await db.SaveChangesAsync(ct);
    }
}

