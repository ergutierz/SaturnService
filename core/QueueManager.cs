using System.Threading.Channels;

namespace SaturnService;

public class QueueManager<T>(ILogger<QueueManager<T>> logger)
{
    private readonly Channel<T> channel = Channel.CreateUnbounded<T>();

    public async Task EnqueueAsync(T item)
    {
        await channel.Writer.WriteAsync(item);
        logger.LogInformation($"Item enqueued: {item}");
    }

    public async Task StartProcessingAsync(Func<T, Task> process, CancellationToken cancellationToken)
    {
        await foreach (var item in channel.Reader.ReadAllAsync(cancellationToken))
        {
            try
            {
                logger.LogInformation($"Processing item: {item}");
                await process(item);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error processing item: {item}");
            }
        }
    }
}