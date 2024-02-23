using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace SaturnService
{
    /// <summary>
    /// Manages a queue of tasks for processing, utilizing a System.Threading.Channels.Channel for concurrency-safe operations.
    /// </summary>
    /// <typeparam name="T">The type of items to be queued and processed.</typeparam>
    public class QueueManager<T>
    {
        private readonly Channel<T> _channel = Channel.CreateUnbounded<T>();
        private readonly ILogger<QueueManager<T>> _logger;

        /// <summary>
        /// Initializes a new instance of the QueueManager class.
        /// </summary>
        /// <param name="logger">The logger for capturing log information related to queue operations.</param>
        public QueueManager(ILogger<QueueManager<T>> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Enqueues an item for processing.
        /// </summary>
        /// <param name="item">The item to be enqueued.</param>
        /// <returns>A Task that represents the asynchronous enqueue operation.</returns>
        public async Task EnqueueAsync(T item)
        {
            await _channel.Writer.WriteAsync(item);
            _logger.LogInformation($"Item enqueued: {item}");
        }

        /// <summary>
        /// Starts processing items in the queue asynchronously.
        /// </summary>
        /// <param name="process">A function that processes an item from the queue.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A Task that represents the asynchronous processing operation.</returns>
        public async Task StartProcessingAsync(Func<T, Task> process, CancellationToken cancellationToken)
        {
            await foreach (var item in _channel.Reader.ReadAllAsync(cancellationToken))
            {
                try
                {
                    _logger.LogInformation($"Processing item: {item}");
                    await process(item);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing item: {item}");
                }
            }
        }
    }
}
