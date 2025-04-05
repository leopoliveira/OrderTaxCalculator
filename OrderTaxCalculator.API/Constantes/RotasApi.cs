namespace OrderTaxCalculator.API.Constantes;

public static class RotasApi
{
    private const string Api = "api";
    private const string V1 = "v1";
    private const string Barra = "/";
    private const string Base = Api + Barra + V1;

    public static class Pedidos
    {
        public const string Rota = Base + "/pedidos";
    }

    public static class DevToken
    {
        public const string Rota = Base + "/dev-token";
    }
}