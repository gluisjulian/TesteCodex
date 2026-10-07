using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Perfumes.Api.Entities;
namespace Perfumes.Api.Configurations;
public sealed class InsumoConfiguration : IEntityTypeConfiguration<Insumo>
{
    public void Configure(EntityTypeBuilder<Insumo> b)
    {
        b.ToTable("Insumos", t => {
            t.HasCheckConstraint("CK_Insumo_Densidade", "[Densidade] IS NULL OR [Densidade] > 0");
            t.HasCheckConstraint("CK_Insumo_Unidade", "[UnidadeMedida] IN (1,2,3)");
            t.HasCheckConstraint("CK_Insumo_Tipo", "[TipoInsumo] IN (1,2)");
        });
        b.HasKey(x => x.Id);
        b.Property(x => x.Nome).HasMaxLength(200).IsRequired();
        b.Property(x => x.Descricao).HasMaxLength(2000);
        b.Property(x => x.Observacao).HasMaxLength(2000);
        b.Property(x => x.Densidade).HasPrecision(18,6);
        b.HasIndex(x => x.Nome);
    }
}

