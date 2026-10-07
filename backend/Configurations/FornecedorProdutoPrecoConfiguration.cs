using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Perfumes.Api.Entities;
namespace Perfumes.Api.Configurations;
public sealed class FornecedorProdutoPrecoConfiguration : IEntityTypeConfiguration<FornecedorProdutoPreco>
{
    public void Configure(EntityTypeBuilder<FornecedorProdutoPreco> b)
    {
        b.ToTable("FornecedorProdutoPrecos", t => {
            t.HasCheckConstraint("CK_Preco_Valor", "[Preco] > 0");
            t.HasCheckConstraint("CK_Preco_Vigencia", "[DataFim] IS NULL OR [DataFim] > [DataInicio]");
        });
        b.HasKey(x => x.Id);
        b.Property(x => x.Preco).HasPrecision(18,2);
        b.HasOne(x => x.FornecedorProduto).WithMany(x => x.Precos).HasForeignKey(x => x.FornecedorProdutoId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => x.FornecedorProdutoId).IsUnique().HasFilter("[DataFim] IS NULL");
        b.HasIndex(x => new { x.FornecedorProdutoId, x.DataInicio }).IsUnique();
    }
}

