using Microsoft.FeatureManagement;
using OrderTaxCalculator.Domain.Constants;
using OrderTaxCalculator.Domain.Entities;
using OrderTaxCalculator.Domain.Enumerator;
using OrderTaxCalculator.Domain.Interfaces.Repositories;
using OrderTaxCalculator.Domain.Interfaces.Services;
using OrderTaxCalculator.Domain.Strategy;

namespace OrderTaxCalculator.Domain.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IFeatureManager _featureManager;

    public PedidoService
    (
        IPedidoRepository pedidoRepository,
        IFeatureManager featureManager
    )
    {
        _pedidoRepository = pedidoRepository;
        _featureManager = featureManager;
    }
    
    public async Task<Pedido?> GetPedidoByIdAsync(long id)
    {
        return await _pedidoRepository
            .GetByIdAsync(id);
    }
    
    public async Task<IReadOnlyList<Pedido>> GetPedidoByStatus(string status)
    {
        var isEnumParsed = Enum.TryParse(status, true, out StatusEnum statusEnum);

        if (!isEnumParsed)
        {
            return [];
        }
        
        return await _pedidoRepository
            .GetByFilter(x => x.Status == statusEnum);
    }

    public async Task<bool> PedidoExiste(long id)
    {
        return await _pedidoRepository.PedidoExisteAsync(id);
    }

    public async Task<Pedido> CreatePedidoAsync(Pedido pedido)
    {
        var pedidoExiste = await PedidoExiste(pedido.PedidoId);

        if (pedidoExiste)
        {
            throw new Exception("Pedido duplicado!");
        }
        
        pedido.SetStatus(StatusEnum.Criado);
        await CalculeImpostoParaPedido(pedido);
        
        await _pedidoRepository.AddAsync(pedido);
        await _pedidoRepository.SaveChangesAsync();
        return pedido;
    }

    public async Task UpdatePedidoAsync(Pedido pedido)
    {
        var data = await _pedidoRepository.GetByIdAsync(pedido.Id);
        if (data == null)
        {
            throw new NullReferenceException("Pedido");
        }

        await _pedidoRepository.UpdateAsync(pedido);
        await _pedidoRepository.SaveChangesAsync();
    }

    public async Task DeletePedidoAsync(Pedido pedido)
    {
        _pedidoRepository.Delete(pedido);
        await _pedidoRepository.SaveChangesAsync();
    }
    
    private async Task CalculeImpostoParaPedido(Pedido pedido)
    {
        ICalcularImpostoService calcularImposto = await GetCalcularImpostoService();

        var totalItens = pedido.TotalItens;
        var imposto = calcularImposto.CalcularImposto(totalItens);
        pedido.SetImposto(imposto);
    }
    
    private async Task<ICalcularImpostoService> GetCalcularImpostoService()
    {
        return await _featureManager.IsEnabledAsync(DomainConstants.ImpostoReformaTributariaFeatureFlag) ?
            new CalcularImpostoServiceStrategy() :
            new CalcularImpostoReformaTributariaStrategy();
    }
}