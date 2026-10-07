namespace Perfumes.Api.Entities;
public sealed class FornecedorProduto : Registro
{
    public long FornecedorId { get; set; }
    public long InsumoId { get; set; }
    public string? CodigoProdutoFornecedor { get; set; }
    public decimal QuantidadeEmbalagem { get; set; }
    public UnidadeMedida UnidadeEmbalagem { get; set; }
    public decimal QuantidadeBase { get; set; }
    public UnidadeMedida UnidadeBase { get; set; }
    public Fornecedor Fornecedor { get; set; } = null!;
    public Insumo Insumo { get; set; } = null!;
    public ICollection<FornecedorProdutoPreco> Precos { get; set; } = new List<FornecedorProdutoPreco>();
}

