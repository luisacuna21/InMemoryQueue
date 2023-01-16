using ccsnQueueTest.IQueues;

namespace ccsnQueueTest;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IBackgroundTaskQueue _taskQueue;

    public Worker(ILogger<Worker> logger, IBackgroundTaskQueue taskQueue)
    {
        _logger = logger;
        _taskQueue = taskQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await ProcessTaskQueueAsync(stoppingToken);
            // await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ProcessTaskQueueAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                Func<CancellationToken, ValueTask> work = await _taskQueue.DequeueAsync(token);
                await work(token);
            }
            catch (OperationCanceledException)
            {
                 // TODO
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while processing task queue");
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker stopping at: {time}", DateTimeOffset.Now);
        await base.StopAsync(cancellationToken);
    }
}
