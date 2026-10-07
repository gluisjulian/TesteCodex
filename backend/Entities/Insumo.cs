namespace Perfumes.Api.Entities;
public sealed class Insumo : Registro
{
    public string Nome { get; set; } = "";
    public string? Descricao { get; set; }
    public TipoInsumo TipoInsumo { get; set; }
    public UnidadeMedida UnidadeMedida { get; set; }
    public decimal? Densidade { get; set; }
    public string? Observacao { get; set; }
    public ICollection<FornecedorProduto> Ofertas { get; set; } = new List<FornecedorProduto>();
}

