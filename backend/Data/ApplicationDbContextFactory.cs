using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace Perfumes.Api.Data;
public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Somente metadados do provedor são necessários para gerar migration/script, sem conexão.
        // A conexão efetiva é sempre configurada externamente para aplicação e database update.
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var conexao = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        if (string.IsNullOrWhiteSpace(conexao)) builder.UseSqlServer();
        else builder.UseSqlServer(conexao);
        return new ApplicationDbContext(builder.Options);
    }
}

