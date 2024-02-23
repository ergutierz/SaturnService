namespace SaturnService
{
    /// <summary>
    /// Background service for processing queued team tasks.
    /// </summary>
    public class QueueBackgroundService : BackgroundService
    {
        private readonly TeamManager _teamManager;
        private readonly QueueManager<TeamManager.TeamTask> _queueManager;
        private readonly ILogger<QueueBackgroundService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueBackgroundService"/> class.
        /// </summary>
        /// <param name="teamManager">The team manager to process team data.</param>
        /// <param name="queueManager">The queue manager to handle task queuing and processing.</param>
        /// <param name="logger">The logger for capturing log information.</param>
        public QueueBackgroundService(
            TeamManager teamManager,
            QueueManager<TeamManager.TeamTask> queueManager,
            ILogger<QueueBackgroundService> logger)
        {
            _teamManager = teamManager;
            _queueManager = queueManager;
            _logger = logger;
        }

        /// <summary>
        /// Executes the background service task.
        /// </summary>
        /// <param name="stoppingToken">A <see cref="CancellationToken"/> used to propagate notification that operations should be canceled.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("QueueBackgroundService is starting.");

            stoppingToken.Register(() => _logger.LogInformation("QueueBackgroundService is stopping."));

            await _queueManager.StartProcessingAsync(async (task) =>
            {
                try
                {
                    _logger.LogInformation($"Processing team number {task.TeamNumber} with correlation ID {task.CorrelationId}.");
                    await _teamManager.ProcessTeamDataAsync(task.TeamNumber, task.CorrelationId);
                    _logger.LogInformation($"Successfully processed team number {task.TeamNumber} with correlation ID {task.CorrelationId}.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing team number {task.TeamNumber} with correlation ID {task.CorrelationId}.");
                }
            }, stoppingToken);

            _logger.LogInformation("QueueBackgroundService has stopped.");
        }
    }
}
