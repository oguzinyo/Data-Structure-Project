using BlockchainAnalysis.Api.Dtos;
using BlockchainAnalysis.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainAnalysis.Api.Controllers;

[ApiController]
[Route("api/analysis")]
[Produces("application/json")]
public class AnalysisController : ControllerBase
{
    private readonly BlockchainService _service;

    public AnalysisController(BlockchainService service)
    {
        _service = service;
    }

    /// <summary>
    /// Belirtilen cuzdandan ileriye donuk fon akisini BFS/DFS sirasiyla dondurur.
    /// </summary>
    /// <param name="startAddress">Baslangic cuzdan adresi (ornek: WALLET_1)</param>
    /// <param name="minAmount">Opsiyonel minimum islem miktari filtresi</param>
    [HttpGet("flow")]
    public ActionResult<FlowResultDto> GetForwardFlow(
        [FromQuery] string startAddress,
        [FromQuery] decimal? minAmount = null)
    {
        if (string.IsNullOrWhiteSpace(startAddress))
            return BadRequest(new { error = "startAddress parametresi zorunludur." });

        try
        {
            var result = _service.GetForwardFlow(startAddress, minAmount);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"Cuzdan adresi bulunamadi: {startAddress}" });
        }
    }

    /// <summary>
    /// Belirtilen cuzdana gelen fonlarin kaynagini geriye donuk izler.
    /// </summary>
    /// <param name="startAddress">Hedef cuzdan adresi</param>
    /// <param name="minAmount">Opsiyonel minimum islem miktari filtresi</param>
    [HttpGet("flow/backward")]
    public ActionResult<FlowResultDto> GetBackwardFlow(
        [FromQuery] string startAddress,
        [FromQuery] decimal? minAmount = null)
    {
        if (string.IsNullOrWhiteSpace(startAddress))
            return BadRequest(new { error = "startAddress parametresi zorunludur." });

        try
        {
            var result = _service.GetBackwardFlow(startAddress, minAmount);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"Cuzdan adresi bulunamadi: {startAddress}" });
        }
    }

    /// <summary>
    /// Iki cuzdan arasindaki en kisa yolu (BFS) bulur.
    /// </summary>
    [HttpGet("path")]
    public ActionResult<PathResultDto> FindPath(
        [FromQuery] string startAddress,
        [FromQuery] string targetAddress)
    {
        if (string.IsNullOrWhiteSpace(startAddress) || string.IsNullOrWhiteSpace(targetAddress))
            return BadRequest(new { error = "startAddress ve targetAddress parametreleri zorunludur." });

        var result = _service.FindPath(startAddress, targetAddress);
        return Ok(result);
    }

    /// <summary>
    /// Iki cuzdan arasindaki maksimum kapasite yolunu bulur (Dijkstra varyanti).
    /// </summary>
    [HttpGet("path/max-capacity")]
    public ActionResult<MaxCapacityPathResultDto> FindMaxCapacityPath(
        [FromQuery] string startAddress,
        [FromQuery] string targetAddress)
    {
        if (string.IsNullOrWhiteSpace(startAddress) || string.IsNullOrWhiteSpace(targetAddress))
            return BadRequest(new { error = "startAddress ve targetAddress parametreleri zorunludur." });

        var result = _service.FindMaxCapacityPath(startAddress, targetAddress);
        return Ok(result);
    }

    /// <summary>
    /// Belirtilen cuzdanin dinamik bakiyesini hesaplar (gelen - giden).
    /// </summary>
    [HttpGet("balance")]
    public ActionResult<BalanceResultDto> GetBalance([FromQuery] string walletAddress)
    {
        if (string.IsNullOrWhiteSpace(walletAddress))
            return BadRequest(new { error = "walletAddress parametresi zorunludur." });

        try
        {
            var result = _service.GetDynamicBalance(walletAddress);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"Cuzdan adresi bulunamadi: {walletAddress}" });
        }
    }
}
