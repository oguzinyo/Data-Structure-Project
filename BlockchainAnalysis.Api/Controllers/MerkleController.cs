using BlockchainAnalysis.Api.Dtos;
using BlockchainAnalysis.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainAnalysis.Api.Controllers;

[ApiController]
[Route("api/merkle")]
[Produces("application/json")]
public class MerkleController : ControllerBase
{
    private readonly BlockchainService _service;

    public MerkleController(BlockchainService service)
    {
        _service = service;
    }

    /// <summary>
    /// Islemlerin Merkle agacini dondurur. transactionId verilirse,
    /// o islemin proof path'i renklendirilmis olarak isaretlenir.
    /// </summary>
    /// <param name="transactionId">Opsiyonel islem ID'si (proof path icin)</param>
    [HttpGet("tree")]
    public ActionResult<MerkleTreeDto> GetMerkleTree([FromQuery] string? transactionId = null)
    {
        var result = _service.GetMerkleTree(transactionId);
        return Ok(result);
    }

    /// <summary>
    /// Sistemdeki tum islem ID'lerini dondurur.
    /// Merkle tree sorgulari icin kullanilabilir.
    /// </summary>
    [HttpGet("transactions")]
    public ActionResult<List<string>> GetTransactionIds()
    {
        return Ok(_service.GetTransactionIds());
    }
}
