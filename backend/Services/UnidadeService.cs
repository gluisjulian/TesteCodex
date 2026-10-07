using Perfumes.Api.Entities;
namespace Perfumes.Api.Services;
public sealed class UnidadeService
{
    public (decimal Quantidade, UnidadeMedida Unidade) Normalizar(decimal quantidade, UnidadeMedida unidade)
    {
        ValidarDecimal(quantidade, 6, 999999999999.999999m);
        var fator = unidade switch {
            UnidadeMedida.L or UnidadeMedida.KG => 1000m,
            UnidadeMedida.ML or UnidadeMedida.G or UnidadeMedida.UN => 1m,
            _ => throw new RegraException(400, "Unidade inválida.")
        };
        var baseUnidade = unidade switch {
            UnidadeMedida.L => UnidadeMedida.ML, UnidadeMedida.KG => UnidadeMedida.G, _ => unidade
        };
        var normalizada = quantidade * fator;
        ValidarDecimal(normalizada, 6, 999999999999.999999m);
        return (normalizada, baseUnidade);
    }
    public static void ValidarDecimal(decimal valor, int casas, decimal maximo)
    {
        if (valor <= 0 || valor > maximo || decimal.Round(valor, casas) != valor)
            throw new RegraException(400, $"Valor deve ser positivo, até {maximo}, com no máximo {casas} casas decimais.");
    }
    public static void ValidarPreco(decimal valor) => ValidarDecimal(valor, 2, 9999999999999999.99m);
    public static decimal CustoUnitario(decimal preco, decimal quantidadeBase) => decimal.Round(preco / quantidadeBase, 6);
}

