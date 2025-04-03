using System.Collections.ObjectModel;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using OrderTaxCalculator.API.Constantes;
using OrderTaxCalculator.API.Dto.Pedido;
using OrderTaxCalculator.API.Mapeamentos;
using OrderTaxCalculator.Domain.Entities;
using OrderTaxCalculator.Domain.Interfaces.Services;

namespace OrderTaxCalculator.API.Controllers.v1;

[Route(ApiRoutes.Pedidos.Rota)]
[ApiController]
public class PedidoController : Controller
{
    private readonly IPedidoService _pedidoService;
    private readonly ILogger<PedidoController> _logger;
    
    public PedidoController(IPedidoService pedidoService, ILogger<PedidoController> logger)
    {
        _pedidoService = pedidoService;
        _logger = logger;
    }
    
    /// <summary>
    /// Recebe e processa um novo pedido.
    /// </summary>
    /// <param name="request">Dados do novo pedido.</param>
    /// <returns>Retorna a localização do novo pedido criado e um resumo.</returns>
    /// <response code="201">Pedido criado com sucesso.</response>
    /// <response code="400">Dados inválidos na requisição.</response>
    /// <response code="500">Erro interno no servidor.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CriarPedidoResponse>> CriarPedido([FromBody] CriarPedidoRequest request)
    {
        _logger.LogInformation("Recebendo requisição para criar novo pedido para ClienteId: {ClienteId}", request.ClienteId);

        var pedido = request.ToPedido();
        var novoPedido = await _pedidoService.CreatePedidoAsync(pedido);
        var pedidoResponse = novoPedido.ToCriarPedidoResponse();

        _logger.LogInformation("Pedido Id {PedidoId} criado com sucesso com Id interno {IdInterno}", novoPedido.PedidoId, novoPedido.Id);
            
        return CreatedAtAction(nameof(GetPedidoPorId), new { id = pedidoResponse.Id }, pedidoResponse);
    }
    
    /// <summary>
    /// Consulta um pedido específico pelo seu ID interno.
    /// </summary>
    /// <param name="id">O ID interno do pedido a ser consultado.</param>
    /// <returns>Os detalhes do pedido encontrado.</returns>
    /// <response code="200">Pedido encontrado e retornado.</response>
    /// <response code="404">Pedido não encontrado para o ID fornecido.</response>
    /// <response code="500">Erro interno no servidor.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ConsultarPedidoResponse>> GetPedidoPorId([FromRoute] long id)
    {
        _logger.LogInformation("Buscando pedido com Id interno: {Id}", id);
        
        var pedido = await _pedidoService.GetPedidoByIdAsync(id);

        if (pedido == null)
        {
            _logger.LogWarning("Pedido com Id interno {Id} não encontrado.", id);
            return NotFound(new { message = $"Pedido com ID {id} não encontrado." });
        }

        var pedidoResponse = pedido.ToConsultarPedidoResponse();

        _logger.LogInformation("Pedido Id {PedidoId} (Id interno {IdInterno}) encontrado.", pedidoResponse.PedidoId, pedidoResponse.Id);
        
        return Ok(pedidoResponse);
    }
    
    /// <summary>
    /// Lista os pedidos, com opção de filtro por status.
    /// </summary>
    /// <param name="status">O status do pedido para filtrar (ex: Criado, Processando).</param>
    /// <returns>Uma lista de pedidos.</returns>
    /// <response code="200">Lista de pedidos retornada com sucesso (pode estar vazia).</response>
    /// <response code="400">Status fornecido para filtro é inválido.</response>
    /// <response code="500">Erro interno no servidor.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ReadOnlyCollection<ConsultarPedidoResponse>>> ListarPedidos([FromQuery] string status)
    {
        _logger.LogInformation("Listando todos os pedidos (sem filtro de status).");
        
        var pedidos = await _pedidoService.GetPedidoByStatus(status);
        var pedidosResponse = MapPedidosToListResponse(pedidos);

        _logger.LogInformation("Retornando {Count} pedidos.", pedidosResponse.Count);
        
        return Ok(pedidosResponse);
    }
    
    private static ReadOnlyCollection<ConsultarPedidoResponse> MapPedidosToListResponse(IEnumerable<Pedido> pedidos)
    {
        return pedidos
            .Select(MapeamentoPedido.ToConsultarPedidoResponse)
            .ToList()
            .AsReadOnly();
    }
}