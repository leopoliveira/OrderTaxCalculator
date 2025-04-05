﻿using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Mvc;
using OrderTaxCalculator.API.Constantes;
using OrderTaxCalculator.API.Dto.Pedido;
using OrderTaxCalculator.API.Mapeamentos;
using OrderTaxCalculator.Domain.Interfaces.Servicos;

namespace OrderTaxCalculator.API.Controllers.v1;

[Route(RotasApi.Pedidos.Rota)]
[Consumes("application/json")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
[ApiController]
public class PedidoController : Controller
{
    private readonly IPedidoServico _pedidoServico;
    private readonly ILogger<PedidoController> _logger;
    
    public PedidoController(IPedidoServico pedidoServico, ILogger<PedidoController> logger)
    {
        _pedidoServico = pedidoServico;
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
    [ProducesResponseType(typeof(CriarPedidoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CriarPedidoResponse>> CriarPedido([FromBody] CriarPedidoRequest request)
    {
        _logger.LogInformation("Recebendo requisição para criar novo pedido para ClienteId: {ClienteId}", request.ClienteId);

        var pedido = request.ParaPedido();
        var novoPedido = await _pedidoServico.CriePedidoAsync(pedido);

        if (novoPedido == null)
        {
            return PedidoDuplicado(request);
        }
        
        var pedidoResponse = novoPedido.ParaCriarPedidoResponse();

        _logger.LogInformation("Pedido Id {PedidoId} criado com sucesso com Id interno {IdInterno}", novoPedido.PedidoId, novoPedido.Id);
            
        return CreatedAtAction(nameof(GetPedidoPorId), new { pedidoId = pedidoResponse.PedidoId }, pedidoResponse);
    }

    /// <summary>
    /// Consulta um pedido específico pelo seu Id.
    /// </summary>
    /// <param name="pedidoId">O Id do pedido a ser consultado.</param>
    /// <returns>Os detalhes do pedido encontrado.</returns>
    /// <response code="200">Pedido encontrado e retornado.</response>
    /// <response code="404">Pedido não encontrado para o ID fornecido.</response>
    /// <response code="500">Erro interno no servidor.</response>
    [HttpGet("{pedidoId:long}")]
    [ProducesResponseType(typeof(ConsultarPedidoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConsultarPedidoResponse>> GetPedidoPorId([FromRoute] long pedidoId)
    {
        _logger.LogInformation("Buscando pedido com Id interno: {Id}", pedidoId);
        
        var pedido = await _pedidoServico.ObtenhaPedidoPorIdAsync(pedidoId);

        if (pedido == null)
        {
            return PedidoNaoEncontrado(pedidoId);
        }

        var pedidoResponse = pedido.ParaConsultarPedidoResponse();

        _logger.LogInformation("Pedido Id {PedidoId} encontrado.", pedidoResponse.PedidoId);
        
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
    [ProducesResponseType(typeof(ReadOnlyCollection<ConsultarPedidoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReadOnlyCollection<ConsultarPedidoResponse>>> ListarPedidos([FromQuery] string status)
    {
        _logger.LogInformation("Listando todos os pedidos por status {status}", status);
        
        var pedidos = await _pedidoServico.ObtenhaPedidoPorStatusAsync(status);
        var pedidosResponse = pedidos.ParaListConsultarPedidoResponse();

        _logger.LogInformation("Retornando {Count} pedidos.", pedidosResponse.Count);
        
        return Ok(pedidosResponse);
    }
    
    private ActionResult<CriarPedidoResponse> PedidoDuplicado(CriarPedidoRequest request)
    {
        var pedidoId = request.PedidoId;
        _logger.LogInformation("Pedido {PedidoId} duplicado", pedidoId);
    
        var problem = new ProblemDetails
        {
            Title = ConstantesApi.PedidoDuplicado,
            Detail = $"Pedido com ID {pedidoId} já existe.",
            Status = StatusCodes.Status400BadRequest
        };
    
        return BadRequest(problem);
    }
    
    private ActionResult<ConsultarPedidoResponse> PedidoNaoEncontrado(long pedidoId)
    {
        _logger.LogWarning("Pedido com Id {Id} não encontrado.", pedidoId);
    
        var problem = new ProblemDetails
        {
            Title = ConstantesApi.PedidoNaoEncontrado,
            Detail = $"Pedido com ID {pedidoId} não encontrado.",
            Status = StatusCodes.Status404NotFound
        };
    
        return NotFound(problem);
    }
}