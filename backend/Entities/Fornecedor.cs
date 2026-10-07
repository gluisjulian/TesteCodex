namespace Perfumes.Api.Entities;
public sealed class Fornecedor : Registro
{
    public string RazaoSocial { get; set; } = "";
    public string? NomeFantasia { get; set; }
    public string? CpfCnpj { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Site { get; set; }
    public string? Observacao { get; set; }
    public ICollection<FornecedorProduto> Produtos { get; set; } = new List<FornecedorProduto>();
}

