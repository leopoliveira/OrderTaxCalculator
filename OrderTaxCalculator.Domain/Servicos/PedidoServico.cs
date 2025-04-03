using Microsoft.FeatureManagement;
using OrderTaxCalculator.Domain.Constantes;
using OrderTaxCalculator.Domain.Entidades;
using OrderTaxCalculator.Domain.Enumeradores;
using OrderTaxCalculator.Domain.Interfaces.Repositorios;
using OrderTaxCalculator.Domain.Interfaces.Servicos;
using OrderTaxCalculator.Domain.Strategy;

namespace OrderTaxCalculator.Domain.Servicos;

public class PedidoServico : IPedidoServico
{
    private readonly IPedidoRepositorio _pedidoRepositorio;
    private readonly IFeatureManager _featureManager;

    public PedidoServico
    (
        IPedidoRepositorio pedidoRepositorio,
        IFeatureManager featureManager
    )
    {
        _pedidoRepositorio = pedidoRepositorio;
        _featureManager = featureManager;
    }
    
    public async Task<Pedido?> ObtenhaPedidoPorIdAsync(long pedidoId)
    {
        return await _pedidoRepositorio
            .ObtenhaPorIdAsync(pedidoId);
    }
    
    public async Task<IReadOnlyList<Pedido>> ObtenhaPedidoPorStatusAsync(string status)
    {
        var isEnumParsed = Enum.TryParse(status, true, out StatusEnum statusEnum);

        if (!isEnumParsed)
        {
            return [];
        }
        
        return await _pedidoRepositorio
            .ObtenhaPorFiltroAsync(x => x.Status == statusEnum);
    }

    public async Task<bool> PedidoExisteAsync(long id)
    {
        return await _pedidoRepositorio.PedidoExisteAsync(id);
    }

    public async Task<Pedido?> CriePedidoAsync(Pedido pedido)
    {
        var pedidoExiste = await PedidoExisteAsync(pedido.PedidoId);

        if (pedidoExiste)
        {
            return null;
        }
        
        pedido.ApliqueStatus(StatusEnum.Criado);
        await CalculeImpostoParaPedido(pedido);
        
        await _pedidoRepositorio.InsiraAsync(pedido);
        await _pedidoRepositorio.SalveMudancasAsync();
        return pedido;
    }
    
    private async Task CalculeImpostoParaPedido(Pedido pedido)
    {
        ICalcularImpostoServico calcularImposto = await ObtenhaCalcularImpostoServico();

        var totalItens = pedido.ValorTotalItens;
        var imposto = calcularImposto.CalcularImposto(totalItens);
        pedido.ApliqueImposto(imposto);
    }
    
    private async Task<ICalcularImpostoServico> ObtenhaCalcularImpostoServico()
    {
        return await _featureManager.IsEnabledAsync(ConstantesDomain.ImpostoReformaTributariaFeatureFlag) ?
            new CalcularImpostoReformaTributariaStrategy() :
            new CalcularImpostoServicoStrategy();
    }
}