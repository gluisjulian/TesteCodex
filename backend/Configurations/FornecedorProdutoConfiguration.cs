using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Perfumes.Api.Entities;
namespace Perfumes.Api.Configurations;
public sealed class FornecedorProdutoConfiguration : IEntityTypeConfiguration<FornecedorProduto>
{
    public void Configure(EntityTypeBuilder<FornecedorProduto> b)
    {
        b.ToTable("FornecedorProdutos", t => {
            t.HasCheckConstraint("CK_Oferta_Quantidade", "[QuantidadeEmbalagem] > 0 AND [QuantidadeBase] > 0");
            t.HasCheckConstraint("CK_Oferta_Unidades", "[UnidadeEmbalagem] IN (1,2,3,4,5) AND [UnidadeBase] IN (1,2,3)");
        });
        b.HasKey(x => x.Id);
        b.Property(x => x.CodigoProdutoFornecedor).HasMaxLength(100);
        b.Property(x => x.QuantidadeEmbalagem).HasPrecision(18,6);
        b.Property(x => x.QuantidadeBase).HasPrecision(18,6);
        b.HasOne(x => x.Fornecedor).WithMany(x => x.Produtos).HasForeignKey(x => x.FornecedorId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Insumo).WithMany(x => x.Ofertas).HasForeignKey(x => x.InsumoId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => new { x.FornecedorId, x.InsumoId, x.QuantidadeBase, x.UnidadeBase }).IsUnique();
    }
}

