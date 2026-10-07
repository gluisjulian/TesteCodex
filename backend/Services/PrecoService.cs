using Microsoft.EntityFrameworkCore;
using Perfumes.Api.Data;
using Perfumes.Api.DTOs;
using Perfumes.Api.Entities;
namespace Perfumes.Api.Services;
public sealed class PrecoService(ApplicationDbContext db)
{
    private async Task OfertaExiste(long id, CancellationToken ct)
    {
        if (!await db.FornecedorProdutos.AnyAsync(x => x.Id == id, ct)) throw new RegraException(404, "Oferta não encontrada.");
    }
    public async Task<PaginaDto<FornecedorProdutoPrecoResponseDto>> Historico(long id, int pagina, int tamanho, CancellationToken ct)
    {
        FornecedorService.ValidarPaginacao(pagina, tamanho);
        await OfertaExiste(id, ct);
        var q = db.FornecedorProdutoPrecos.AsNoTracking().Where(x => x.FornecedorProdutoId == id);
        var total = await q.CountAsync(ct);
        var itens = await q.OrderByDescending(x => x.DataInicio).ThenByDescending(x => x.Id).Skip((pagina-1)*tamanho).Take(tamanho).ToListAsync(ct);
        return new(itens.Select(x => x.ParaDto()).ToList(), total, pagina, tamanho);
    }
    public async Task<FornecedorProdutoPrecoResponseDto> Atual(long id, CancellationToken ct)
    {
        await OfertaExiste(id, ct);
        return (await db.FornecedorProdutoPrecos.AsNoTracking().SingleOrDefaultAsync(x => x.FornecedorProdutoId == id && x.DataFim == null, ct)
            ?? throw new RegraException(404, "Oferta sem preço vigente.")).ParaDto();
    }
    public async Task<FornecedorProdutoPrecoResponseDto> Alterar(long id, FornecedorProdutoPrecoCreateDto dto, CancellationToken ct)
    {
        UnidadeService.ValidarPreco(dto.Preco);
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, ct);
        // UPDLOCK serializa os escritores por oferta antes de consultar o preço vigente.
        var oferta = await db.FornecedorProdutos.FromSqlInterpolated(
            $"SELECT * FROM [FornecedorProdutos] WITH (UPDLOCK, HOLDLOCK) WHERE [Id] = {id}").SingleOrDefaultAsync(ct)
            ?? throw new RegraException(404, "Oferta não encontrada.");
        if (!oferta.Ativo || !await db.Fornecedores.AnyAsync(x => x.Id == oferta.FornecedorId && x.Ativo, ct)
            || !await db.Insumos.AnyAsync(x => x.Id == oferta.InsumoId && x.Ativo, ct))
            throw new RegraException(409, "Oferta, fornecedor e insumo devem estar ativos para alterar preço.");
        var vigente = await db.FornecedorProdutoPrecos.SingleOrDefaultAsync(x => x.FornecedorProdutoId == id && x.DataFim == null, ct);
        if (vigente?.Preco == dto.Preco) throw new RegraException(409, "O novo preço é igual ao vigente.");
        var agora = DateTimeOffset.UtcNow;
        if (vigente != null)
        {
            if (agora <= vigente.DataInicio) agora = vigente.DataInicio.AddTicks(1);
            vigente.DataFim = agora;
            // Libera o índice único filtrado antes de inserir; ambos os saves pertencem à mesma transação.
            await db.SaveChangesAsync(ct);
        }
        var novo = new FornecedorProdutoPreco { FornecedorProdutoId = id, Preco = dto.Preco, DataInicio = agora, DataCadastro = agora };
        db.FornecedorProdutoPrecos.Add(novo);
        db.Entry(oferta).Property(x => x.DataAtualizacao).IsModified = true;
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return novo.ParaDto();
    }
}

