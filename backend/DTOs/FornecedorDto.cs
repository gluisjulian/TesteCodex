using System.ComponentModel.DataAnnotations;
namespace Perfumes.Api.DTOs;
public sealed class FornecedorSalvarDto
{
    [Required, StringLength(200)] public string RazaoSocial { get; set; } = "";
    [StringLength(200)] public string? NomeFantasia { get; set; }
    [StringLength(18), RegularExpression(@"^(?:\d{11}|\d{14}|\d{3}\.\d{3}\.\d{3}-\d{2}|\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2})$", ErrorMessage = "Informe CPF/CNPJ válido, com ou sem pontuação.")] public string? CpfCnpj { get; set; }
    [StringLength(30)] public string? Telefone { get; set; }
    [EmailAddress, StringLength(254)] public string? Email { get; set; }
    [Url, StringLength(500)] public string? Site { get; set; }
    [StringLength(2000)] public string? Observacao { get; set; }
    public bool Ativo { get; set; } = true;
}
public sealed record FornecedorResponseDto(long Id, string RazaoSocial, string? NomeFantasia,
    string? CpfCnpj, string? Telefone, string? Email, string? Site, string? Observacao,
    bool Ativo, DateTimeOffset DataCadastro, DateTimeOffset DataAtualizacao);

