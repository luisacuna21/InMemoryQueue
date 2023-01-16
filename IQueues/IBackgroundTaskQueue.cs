using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ccsnQueueTest.IQueues;

public interface IBackgroundTaskQueue
{
    ValueTask QueueRemindersAsync(Func<CancellationToken, ValueTask> work);
    ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}
