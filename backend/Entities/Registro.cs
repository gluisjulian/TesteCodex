namespace Perfumes.Api.Entities;
public abstract class Registro
{
    public long Id { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTimeOffset DataCadastro { get; set; }
    public DateTimeOffset DataAtualizacao { get; set; }
}

