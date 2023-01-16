using ccsnQueueTest;
using ccsnQueueTest.IQueues;
using ccsnQueueTest.Queues;
using ccsnQueueTest.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<EventReminderMonitorLoop>();
        services.AddHostedService<Worker>();
        services.AddSingleton<IBackgroundTaskQueue>(_=>{
            if(!int.TryParse(context.Configuration["QueueSize"], out var queueSize))
                queueSize = 100;
            return new EventSuscriptionBackgroundTaskQueue(queueSize);
        });
    })
    .Build();

EventReminderMonitorLoop eventReminderMonitorLoop = host.Services.GetRequiredService<EventReminderMonitorLoop>();
eventReminderMonitorLoop.StartEventReminderMonitorLoop();

await host.RunAsync();
