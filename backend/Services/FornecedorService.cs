using Microsoft.EntityFrameworkCore;
using Perfumes.Api.Data;
using Perfumes.Api.DTOs;
using Perfumes.Api.Entities;
namespace Perfumes.Api.Services;
public sealed class FornecedorService(ApplicationDbContext db)
{
    public async Task<PaginaDto<FornecedorResponseDto>> Listar(string? busca, bool? ativo, int pagina, int tamanho, CancellationToken ct)
    {
        ValidarPaginacao(pagina, tamanho);
        var query = db.Fornecedores.AsNoTracking();
        if (ativo.HasValue) query = query.Where(x => x.Ativo == ativo);
        if (!string.IsNullOrWhiteSpace(busca))
        {
            var termo = busca.Trim();
            var documento = Documento(termo);
            query = query.Where(x => x.RazaoSocial.Contains(termo) ||
                (x.NomeFantasia != null && x.NomeFantasia.Contains(termo)) ||
                (documento != null && x.CpfCnpj != null && x.CpfCnpj.Contains(documento)));
        }
        var total = await query.CountAsync(ct);
        var itens = await query.OrderBy(x => x.RazaoSocial).ThenBy(x => x.Id).Skip((pagina-1)*tamanho).Take(tamanho).ToListAsync(ct);
        return new(itens.Select(x => x.ParaDto()).ToList(), total, pagina, tamanho);
    }
    public async Task<FornecedorResponseDto> Obter(long id, CancellationToken ct) => (await Entidade(id, ct)).ParaDto();
    private async Task<Fornecedor> Entidade(long id, CancellationToken ct) =>
        await db.Fornecedores.SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw new RegraException(404, "Fornecedor não encontrado.");
    public async Task<FornecedorResponseDto> Salvar(long? id, FornecedorSalvarDto dto, CancellationToken ct)
    {
        var doc = Documento(dto.CpfCnpj);
        if (doc != null && doc.Length is not (11 or 14)) throw new RegraException(400, "CPF/CNPJ deve conter 11 ou 14 dígitos.");
        if (doc != null && await db.Fornecedores.AnyAsync(x => x.Id != id && x.CpfCnpj == doc, ct))
            throw new RegraException(409, "CPF/CNPJ já cadastrado.");
        var x = id.HasValue ? await Entidade(id.Value, ct) : new Fornecedor();
        x.RazaoSocial = dto.RazaoSocial.Trim();
        if (x.RazaoSocial.Length == 0) throw new RegraException(400, "Razão social obrigatória.");
        x.NomeFantasia = Limpar(dto.NomeFantasia); x.CpfCnpj = doc; x.Telefone = Limpar(dto.Telefone);
        x.Email = Limpar(dto.Email); x.Site = Limpar(dto.Site); x.Observacao = Limpar(dto.Observacao); x.Ativo = dto.Ativo;
        if (!id.HasValue) db.Fornecedores.Add(x);
        await db.SaveChangesAsync(ct);
        return x.ParaDto();
    }
    public async Task AlterarStatus(long id, bool ativo, CancellationToken ct)
    {
        (await Entidade(id, ct)).Ativo = ativo;
        await db.SaveChangesAsync(ct);
    }
    public static string? Limpar(string? valor) => string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    public static string? Documento(string? valor) => Limpar(new string((valor ?? "").Where(char.IsAsciiDigit).ToArray()));
    public static void ValidarPaginacao(int pagina, int tamanho)
    {
        if (pagina < 1 || tamanho < 1 || tamanho > 100 || pagina > int.MaxValue / tamanho)
            throw new RegraException(400, "Página deve ser positiva e tamanho deve estar entre 1 e 100.");
    }
}

