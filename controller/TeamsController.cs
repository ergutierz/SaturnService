using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace SaturnService;

[ApiController]
[Route("[controller]")]
public class TeamsController(
    QueueManager<TeamManager.TeamTask> queueManager,
    IMemoryCache cache,
    ILogger<TeamsController> logger)
    : ControllerBase
{
    [HttpPost("EnqueueTeam")]
    public async Task<IActionResult> EnqueueTeamDataProcessing([FromBody] TeamRequest request)
    {
        var correlationId = Guid.NewGuid().ToString();
        logger.LogInformation($"Received request to enqueue team {request.TeamNumber} with correlation ID {correlationId}.");

        var item = new TeamManager.TeamTask
        {
            TeamNumber = request.TeamNumber,
            CorrelationId = correlationId
        };

        await queueManager.EnqueueAsync(item);
        logger.LogInformation($"Team {request.TeamNumber} enqueued for processing with correlation ID {correlationId}.");

        return Accepted(new { CorrelationId = correlationId });
    }

    [HttpGet("GetProcessedData/{correlationId}")]
    public IActionResult GetProcessedData(string correlationId)
    {
        logger.LogInformation($"Request received to get processed data with correlation ID {correlationId}.");

        if (cache.TryGetValue(correlationId, out List<TeamManager.TeamStat> teamStats))
        {
            logger.LogInformation($"Processed data found for correlation ID {correlationId}.");
            return Ok(teamStats);
        }
        else
        {
            logger.LogWarning($"Processed data not found or not complete for correlation ID {correlationId}.");
            return NotFound("Data not found or processing not complete.");
        }
    }

}
