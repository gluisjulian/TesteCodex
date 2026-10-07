namespace Perfumes.Api.Entities;
public sealed class FornecedorProdutoPreco
{
    public long Id { get; set; }
    public long FornecedorProdutoId { get; set; }
    public decimal Preco { get; set; }
    public DateTimeOffset DataInicio { get; set; }
    public DateTimeOffset? DataFim { get; set; }
    public DateTimeOffset DataCadastro { get; set; }
    public FornecedorProduto FornecedorProduto { get; set; } = null!;
}

