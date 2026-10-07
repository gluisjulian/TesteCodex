using Microsoft.EntityFrameworkCore;
using Perfumes.Api.Data;
using Perfumes.Api.DTOs;
using Perfumes.Api.Entities;
namespace Perfumes.Api.Services;
public sealed class FornecedorProdutoService(ApplicationDbContext db, UnidadeService unidades)
{
    private async Task<Fornecedor> Fornecedor(long id, CancellationToken ct) =>
        await db.Fornecedores.SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw new RegraException(404, "Fornecedor não encontrado.");
    private IQueryable<FornecedorProduto> Consulta() => db.FornecedorProdutos.Include(x => x.Insumo).Include(x => x.Precos.Where(p => p.DataFim == null));
    public async Task<FornecedorProdutoResponseDto> Obter(long fornecedorId, long id, CancellationToken ct)
    {
        await Fornecedor(fornecedorId, ct);
        return (await Consulta().AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && x.FornecedorId == fornecedorId, ct)
            ?? throw new RegraException(404, "Oferta não encontrada.")).ParaDto();
    }
    public async Task<PaginaDto<FornecedorProdutoResponseDto>> Listar(long fornecedorId, bool? ativo, int pagina, int tamanho, CancellationToken ct)
    {
        FornecedorService.ValidarPaginacao(pagina, tamanho);
        await Fornecedor(fornecedorId, ct);
        var q = Consulta().AsNoTracking().Where(x => x.FornecedorId == fornecedorId);
        if (ativo.HasValue) q = q.Where(x => x.Ativo == ativo);
        var total = await q.CountAsync(ct);
        var itens = await q.OrderBy(x => x.Insumo.Nome).ThenBy(x => x.Id).Skip((pagina-1)*tamanho).Take(tamanho).ToListAsync(ct);
        return new(itens.Select(x => x.ParaDto()).ToList(), total, pagina, tamanho);
    }
    public async Task<FornecedorProdutoResponseDto> Criar(long fornecedorId, FornecedorProdutoCreateDto dto, CancellationToken ct)
    {
        UnidadeService.ValidarPreco(dto.PrecoInicial);
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, ct);
        var fornecedor = await Fornecedor(fornecedorId, ct);
        var insumo = await db.Insumos.SingleOrDefaultAsync(x => x.Id == dto.InsumoId, ct) ?? throw new RegraException(404, "Insumo não encontrado.");
        if (!fornecedor.Ativo || !insumo.Ativo) throw new RegraException(409, "Fornecedor e insumo devem estar ativos.");
        var normal = unidades.Normalizar(dto.QuantidadeEmbalagem, dto.UnidadeEmbalagem);
        if (normal.Unidade != insumo.UnidadeMedida) throw new RegraException(400, "Unidade da apresentação incompatível com o insumo.");
        if (await db.FornecedorProdutos.AnyAsync(x => x.FornecedorId == fornecedorId && x.InsumoId == dto.InsumoId
            && x.QuantidadeBase == normal.Quantidade && x.UnidadeBase == normal.Unidade, ct))
            throw new RegraException(409, "Esta apresentação já está cadastrada; reative a oferta se necessário.");
        var agora = DateTimeOffset.UtcNow;
        var x = new FornecedorProduto {
            FornecedorId = fornecedorId, InsumoId = dto.InsumoId, Insumo = insumo,
            CodigoProdutoFornecedor = FornecedorService.Limpar(dto.CodigoProdutoFornecedor),
            QuantidadeEmbalagem = dto.QuantidadeEmbalagem, UnidadeEmbalagem = dto.UnidadeEmbalagem,
            QuantidadeBase = normal.Quantidade, UnidadeBase = normal.Unidade, Ativo = dto.Ativo
        };
        x.Precos.Add(new FornecedorProdutoPreco { Preco = dto.PrecoInicial, DataInicio = agora, DataCadastro = agora });
        db.FornecedorProdutos.Add(x);
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return x.ParaDto();
    }
    public async Task<FornecedorProdutoResponseDto> Atualizar(long fornecedorId, long id, FornecedorProdutoSalvarDto dto, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, ct);
        var x = await Consulta().SingleOrDefaultAsync(x => x.Id == id && x.FornecedorId == fornecedorId, ct)
            ?? throw new RegraException(404, "Oferta não encontrada.");
        if (x.InsumoId != dto.InsumoId || x.QuantidadeEmbalagem != dto.QuantidadeEmbalagem || x.UnidadeEmbalagem != dto.UnidadeEmbalagem)
            throw new RegraException(409, "Crie outra oferta para uma apresentação ou insumo diferente, preservando o histórico.");
        x.CodigoProdutoFornecedor = FornecedorService.Limpar(dto.CodigoProdutoFornecedor);
        x.Ativo = dto.Ativo;
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return x.ParaDto();
    }
    public async Task AlterarStatus(long fornecedorId, long id, bool ativo, CancellationToken ct)
    {
        var x = await db.FornecedorProdutos.SingleOrDefaultAsync(x => x.Id == id && x.FornecedorId == fornecedorId, ct)
            ?? throw new RegraException(404, "Oferta não encontrada.");
        x.Ativo = ativo;
        await db.SaveChangesAsync(ct);
    }
}

