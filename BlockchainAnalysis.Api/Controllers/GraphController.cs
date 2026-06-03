using BlockchainAnalysis.Api.Dtos;
using BlockchainAnalysis.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainAnalysis.Api.Controllers;

[ApiController]
[Route("api/graph")]
[Produces("application/json")]
public class GraphController : ControllerBase
{
    private readonly BlockchainService _service;

    public GraphController(BlockchainService service)
    {
        _service = service;
    }

    /// <summary>
    /// Tum graf verisini (dugumler + kenarlar) dondurur.
    /// Frontend grafik motorunun dogrudan tuketebilecegi formatta.
    /// </summary>
    [HttpGet]
    public ActionResult<GraphDataDto> GetFullGraph()
    {
        return Ok(_service.GetGraphData());
    }

    /// <summary>
    /// Tum cuzdan dugumlerini, adreslerini ve guncel bakiyelerini dondurur.
    /// </summary>
    [HttpGet("nodes")]
    public ActionResult<List<WalletNodeDto>> GetNodes()
    {
        return Ok(_service.GetAllNodes());
    }

    /// <summary>
    /// Tum islem kenarlarini (transferler, miktarlar, zaman damgalari) dondurur.
    /// </summary>
    [HttpGet("edges")]
    public ActionResult<List<TransactionEdgeDto>> GetEdges()
    {
        return Ok(_service.GetAllEdges());
    }
}
