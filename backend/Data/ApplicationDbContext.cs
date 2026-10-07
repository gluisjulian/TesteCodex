using Microsoft.EntityFrameworkCore;
using Perfumes.Api.Entities;
namespace Perfumes.Api.Data;
public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<Insumo> Insumos => Set<Insumo>();
    public DbSet<FornecedorProduto> FornecedorProdutos => Set<FornecedorProduto>();
    public DbSet<FornecedorProdutoPreco> FornecedorProdutoPrecos => Set<FornecedorProdutoPreco>();
    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var agora = DateTimeOffset.UtcNow;
        foreach (var entry in ChangeTracker.Entries<Registro>())
        {
            if (entry.State == EntityState.Added) entry.Entity.DataCadastro = agora;
            if (entry.State is EntityState.Added or EntityState.Modified) entry.Entity.DataAtualizacao = agora;
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}

