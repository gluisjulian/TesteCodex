using System.ComponentModel.DataAnnotations;
namespace Perfumes.Api.DTOs;
public sealed class FornecedorProdutoPrecoCreateDto
{
    [Range(typeof(decimal), "0.01", "9999999999999999.99")] public decimal Preco { get; set; }
}
public sealed record FornecedorProdutoPrecoResponseDto(long Id, long FornecedorProdutoId, decimal Preco,
    DateTimeOffset DataInicio, DateTimeOffset? DataFim, DateTimeOffset DataCadastro);

