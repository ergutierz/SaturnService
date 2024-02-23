using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace SaturnService
{
    /// <summary>
    /// Controller responsible for managing team data operations.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly QueueManager<TeamManager.TeamTask> _queueManager;
        private readonly IMemoryCache _cache;
        private readonly ILogger<TeamsController> _logger;

        /// <summary>
        /// Initializes a new instance of the TeamsController class.
        /// </summary>
        /// <param name="queueManager">The queue manager for handling team tasks.</param>
        /// <param name="cache">The memory cache for storing processed data.</param>
        /// <param name="logger">The logger for capturing log information.</param>
        public TeamsController(
            QueueManager<TeamManager.TeamTask> queueManager,
            IMemoryCache cache,
            ILogger<TeamsController> logger)
        {
            _queueManager = queueManager;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Enqueues a team data processing request.
        /// </summary>
        /// <param name="request">The team request containing the team number.</param>
        /// <returns>An accepted action result with the generated correlation ID.</returns>
        [HttpPost("EnqueueTeam")]
        public async Task<IActionResult> EnqueueTeamDataProcessing([FromBody] TeamRequest request)
        {
            var correlationId = Guid.NewGuid().ToString();
            _logger.LogInformation($"Received request to enqueue team {request.TeamNumber} with correlation ID {correlationId}.");

            var item = new TeamManager.TeamTask
            {
                TeamNumber = request.TeamNumber,
                CorrelationId = correlationId
            };

            await _queueManager.EnqueueAsync(item);
            _logger.LogInformation($"Team {request.TeamNumber} enqueued for processing with correlation ID {correlationId}.");

            return Accepted(new { CorrelationId = correlationId });
        }

        /// <summary>
        /// Retrieves processed team data for a given correlation ID.
        /// </summary>
        /// <param name="correlationId">The correlation ID associated with the request.</param>
        /// <returns>An action result containing the processed team data or a not found result.</returns>
        [HttpGet("GetProcessedData/{correlationId}")]
        public IActionResult GetProcessedData(string correlationId)
        {
            _logger.LogInformation($"Request received to get processed data with correlation ID {correlationId}.");

            if (_cache.TryGetValue(correlationId, out List<TeamManager.TeamStat> teamStats))
            {
                _logger.LogInformation($"Processed data found for correlation ID {correlationId}.");
                return Ok(teamStats);
            }
            else
            {
                _logger.LogWarning($"Processed data not found or not complete for correlation ID {correlationId}.");
                return NotFound("Data not found or processing not complete.");
            }
        }
    }
}
