using System.Threading.Channels;

namespace SaturnService;

using Microsoft.Extensions.Logging;

public class QueueBackgroundService(
    TeamManager teamManager,
    QueueManager<TeamManager.TeamTask> queueManager,
    ILogger<QueueBackgroundService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("QueueBackgroundService is starting.");

        stoppingToken.Register(() => logger.LogInformation("QueueBackgroundService is stopping."));

        await queueManager.StartProcessingAsync(async (task) =>
        {
            try
            {
                logger.LogInformation($"Processing team number {task.TeamNumber} with correlation ID {task.CorrelationId}.");
                await teamManager.ProcessTeamDataAsync(task.TeamNumber, task.CorrelationId);
                logger.LogInformation($"Successfully processed team number {task.TeamNumber} with correlation ID {task.CorrelationId}.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error processing team number {task.TeamNumber} with correlation ID {task.CorrelationId}.");
            }
        }, stoppingToken);

        logger.LogInformation("QueueBackgroundService has stopped.");
    }
}


