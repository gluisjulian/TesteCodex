using System.ComponentModel.DataAnnotations;
using Perfumes.Api.Entities;
namespace Perfumes.Api.DTOs;
public class FornecedorProdutoSalvarDto
{
    [Range(1, long.MaxValue)] public long InsumoId { get; set; }
    [StringLength(100)] public string? CodigoProdutoFornecedor { get; set; }
    [Range(typeof(decimal), "0.000001", "999999999999.999999")] public decimal QuantidadeEmbalagem { get; set; }
    [EnumDataType(typeof(UnidadeMedida))] public UnidadeMedida UnidadeEmbalagem { get; set; }
    public bool Ativo { get; set; } = true;
}
public sealed class FornecedorProdutoCreateDto : FornecedorProdutoSalvarDto
{
    [Range(typeof(decimal), "0.01", "9999999999999999.99")] public decimal PrecoInicial { get; set; }
}
public sealed record FornecedorProdutoResponseDto(long Id, long FornecedorId, long InsumoId,
    string InsumoNome, string? CodigoProdutoFornecedor, decimal QuantidadeEmbalagem,
    UnidadeMedida UnidadeEmbalagem, decimal QuantidadeBase, UnidadeMedida UnidadeBase,
    bool Ativo, decimal? PrecoAtual, decimal? CustoUnidadeBase, DateTimeOffset? DataPrecoAtual,
    DateTimeOffset DataCadastro, DateTimeOffset DataAtualizacao);

