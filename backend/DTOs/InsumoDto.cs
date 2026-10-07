using System.ComponentModel.DataAnnotations;
using Perfumes.Api.Entities;
namespace Perfumes.Api.DTOs;
public sealed class InsumoSalvarDto
{
    [Required, StringLength(200)] public string Nome { get; set; } = "";
    [StringLength(2000)] public string? Descricao { get; set; }
    [EnumDataType(typeof(TipoInsumo))] public TipoInsumo TipoInsumo { get; set; }
    [EnumDataType(typeof(UnidadeMedida))] public UnidadeMedida UnidadeMedida { get; set; }
    [Range(typeof(decimal), "0.000001", "999999999999.999999")] public decimal? Densidade { get; set; }
    [StringLength(2000)] public string? Observacao { get; set; }
    public bool Ativo { get; set; } = true;
}
public sealed record InsumoResponseDto(long Id, string Nome, string? Descricao, TipoInsumo TipoInsumo,
    UnidadeMedida UnidadeMedida, decimal? Densidade, string? Observacao, bool Ativo,
    DateTimeOffset DataCadastro, DateTimeOffset DataAtualizacao);

