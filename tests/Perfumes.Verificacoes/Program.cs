using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Perfumes.Api.Data;
using Perfumes.Api.DTOs;
using Perfumes.Api.Entities;
using Perfumes.Api.Services;
var total = 0;
void Check(bool ok, string name) { if (!ok) throw new Exception(name); total++; }
void Reject(Action action, string name) {
    try { action(); } catch (RegraException e) when(e.Status == 400) { total++; return; }
    throw new Exception(name);
}
var units = new UnidadeService();
Check(units.Normalizar(1, UnidadeMedida.L) == (1000m, UnidadeMedida.ML), "1 litro");
Check(units.Normalizar(5, UnidadeMedida.KG) == (5000m, UnidadeMedida.G), "5 kg");
Check(units.Normalizar(1, UnidadeMedida.L) == units.Normalizar(1000, UnidadeMedida.ML), "Apresentações equivalentes");
Check(UnidadeService.CustoUnitario(25, 1000) == 0.025m, "Custo ML A");
Check(UnidadeService.CustoUnitario(110, 5000) == 0.022m, "Custo ML B");
Reject(() => units.Normalizar(0, UnidadeMedida.ML), "Quantidade zero");
Reject(() => units.Normalizar(-1, UnidadeMedida.ML), "Quantidade negativa");
Reject(() => units.Normalizar(1, (UnidadeMedida)99), "Enum inválido");
Reject(() => units.Normalizar(0.0000001m, UnidadeMedida.ML), "Precisão quantidade");
Reject(() => units.Normalizar(999999999999m, UnidadeMedida.L), "Overflow na normalização");
Reject(() => UnidadeService.ValidarPreco(10.001m), "Precisão monetária");
Check(FornecedorService.Documento("12.345.678/0001-90") == "12345678000190", "Normalização documento");
Check(FornecedorService.Documento("") == null, "Documento opcional");
var validation = new List<ValidationResult>();
var invalid = new FornecedorSalvarDto { RazaoSocial = "Teste", CpfCnpj = "abc" };
Check(!Validator.TryValidateObject(invalid, new ValidationContext(invalid), validation, true), "Documento inválido");
using var db = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer().Options);
var model = db.Model;
Check(model.GetEntityTypes().Count() == 4, "Quatro entidades");
Check(model.FindEntityType(typeof(Fornecedor))!.GetIndexes().Any(x => x.IsUnique && x.GetFilter() == "[CpfCnpj] IS NOT NULL"), "Documento único");
var offer = model.FindEntityType(typeof(FornecedorProduto))!;
Check(offer.GetIndexes().Any(x => x.IsUnique && x.Properties.Select(p => p.Name).SequenceEqual(new[] { "FornecedorId", "InsumoId", "QuantidadeBase", "UnidadeBase" })), "Apresentação única");
var price = model.FindEntityType(typeof(FornecedorProdutoPreco))!;
Check(price.GetIndexes().Any(x => x.IsUnique && x.GetFilter() == "[DataFim] IS NULL"), "Um preço vigente");
Check(price.FindProperty("Preco")!.GetPrecision() == 18 && price.FindProperty("Preco")!.GetScale() == 2, "Precisão SQL preço");
Check(model.GetEntityTypes().SelectMany(x => x.GetForeignKeys()).All(x => x.DeleteBehavior == DeleteBehavior.Restrict), "Histórico protegido de cascata");
Console.WriteLine($"{total} verificações passaram.");

