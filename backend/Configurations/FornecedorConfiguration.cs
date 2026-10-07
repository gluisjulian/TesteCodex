using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Perfumes.Api.Entities;
namespace Perfumes.Api.Configurations;
public sealed class FornecedorConfiguration : IEntityTypeConfiguration<Fornecedor>
{
    public void Configure(EntityTypeBuilder<Fornecedor> b)
    {
        b.ToTable("Fornecedores");
        b.HasKey(x => x.Id);
        b.Property(x => x.RazaoSocial).HasMaxLength(200).IsRequired();
        b.Property(x => x.NomeFantasia).HasMaxLength(200);
        b.Property(x => x.CpfCnpj).HasMaxLength(14);
        b.Property(x => x.Telefone).HasMaxLength(30);
        b.Property(x => x.Email).HasMaxLength(254);
        b.Property(x => x.Site).HasMaxLength(500);
        b.Property(x => x.Observacao).HasMaxLength(2000);
        b.HasIndex(x => x.CpfCnpj).IsUnique().HasFilter("[CpfCnpj] IS NOT NULL");
        b.HasIndex(x => x.RazaoSocial);
    }
}

