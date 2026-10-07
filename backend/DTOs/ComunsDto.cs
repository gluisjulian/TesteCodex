namespace Perfumes.Api.DTOs;
public sealed record StatusDto(bool Ativo);
public sealed record PaginaDto<T>(IReadOnlyList<T> Itens, int Total, int Pagina, int TamanhoPagina);

