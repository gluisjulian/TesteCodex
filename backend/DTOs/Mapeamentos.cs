using Perfumes.Api.Entities;
using Perfumes.Api.Services;
namespace Perfumes.Api.DTOs;
public static class Mapeamentos
{
    public static FornecedorResponseDto ParaDto(this Fornecedor x) => new(x.Id, x.RazaoSocial, x.NomeFantasia,
        x.CpfCnpj, x.Telefone, x.Email, x.Site, x.Observacao, x.Ativo, x.DataCadastro, x.DataAtualizacao);
    public static InsumoResponseDto ParaDto(this Insumo x) => new(x.Id, x.Nome, x.Descricao, x.TipoInsumo,
        x.UnidadeMedida, x.Densidade, x.Observacao, x.Ativo, x.DataCadastro, x.DataAtualizacao);
    public static FornecedorProdutoPrecoResponseDto ParaDto(this FornecedorProdutoPreco x) =>
        new(x.Id, x.FornecedorProdutoId, x.Preco, x.DataInicio, x.DataFim, x.DataCadastro);
    public static FornecedorProdutoResponseDto ParaDto(this FornecedorProduto x)
    {
        var atual = x.Precos.SingleOrDefault(p => p.DataFim == null);
        return new(x.Id, x.FornecedorId, x.InsumoId, x.Insumo.Nome, x.CodigoProdutoFornecedor,
            x.QuantidadeEmbalagem, x.UnidadeEmbalagem, x.QuantidadeBase, x.UnidadeBase, x.Ativo,
            atual?.Preco, atual == null ? null : UnidadeService.CustoUnitario(atual.Preco, x.QuantidadeBase),
            atual?.DataInicio, x.DataCadastro, x.DataAtualizacao);
    }
}

