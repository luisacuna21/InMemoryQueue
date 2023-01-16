using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using ccsnQueueTest.IQueues;

namespace ccsnQueueTest.Queues;

public class EventSuscriptionBackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

    public EventSuscriptionBackgroundTaskQueue(int capacity)
    {
        BoundedChannelOptions options = new(capacity){
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
    }

    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
    {
        Func<CancellationToken, ValueTask>? work = await _queue.Reader.ReadAsync(cancellationToken);
        return work;
    }

    public async ValueTask QueueRemindersAsync(Func<CancellationToken, ValueTask> work)
    {
        ArgumentNullException.ThrowIfNull(work);
        await _queue.Writer.WriteAsync(work);
    }
}
