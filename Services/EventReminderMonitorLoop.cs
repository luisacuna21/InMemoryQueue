using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ccsnQueueTest.IQueues;
using inMemoryQueue.ccsnQueueTest.Utils;

namespace ccsnQueueTest.Services;

public class EventReminderMonitorLoop
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ILogger<EventReminderMonitorLoop> _logger;
    private readonly CancellationToken _cancellationToken;
    private UpcomingEvent UpcomingEvents { get; set; }

    public EventReminderMonitorLoop(IBackgroundTaskQueue taskQueue, ILogger<EventReminderMonitorLoop> logger, IHostApplicationLifetime applicationLifetime)
    {
        _taskQueue = taskQueue;
        _logger = logger;
        _cancellationToken = applicationLifetime.ApplicationStopping;
        UpcomingEvents = new UpcomingEvent();
    }

    public void StartEventReminderMonitorLoop()
    {
        _logger.LogInformation($"{nameof(MonitorAsync)} loop is starting.");

        Task.Run(async () => await MonitorAsync());
    }

    private async ValueTask MonitorAsync()
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            var count = UpcomingEvents.GetEventSuscriptionReminders().Count();

            if (count > 0)
            {
                await _taskQueue.QueueRemindersAsync(BuildEventReminderAsync);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), _cancellationToken);
        }
    }

    private async ValueTask BuildEventReminderAsync(CancellationToken arg)
    {
        var events = UpcomingEvents.GetUpcomingEvents();
        var eventSuscriptionReminders = UpcomingEvents.GetEventSuscriptionReminders(events);
        int totalRemindersToSend = eventSuscriptionReminders.Count();

        int releasedReminders = 0;

        _logger.LogInformation($"Sending event reminder to suscribed users.{Environment.NewLine}{eventSuscriptionReminders.Count()} reminders will be sent.{Environment.NewLine}");

        await Task.Delay(TimeSpan.FromSeconds(1));

        foreach (var er in eventSuscriptionReminders)
        {
            if(er.EventSuscription.EventReminderSent)
            {
                _logger.LogInformation($"Titled event reminder: {er.Event.Title} already sent to subscribed users.{Environment.NewLine}");
                continue;
            }

            var guid = Guid.NewGuid();
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(2), arg);
                _logger.LogInformation($"Sending reminder operation with Id: {guid}.{Environment.NewLine}");
                _logger.LogInformation($"{Environment.NewLine}Sending event reminder to {er.User.UserName} with Id: {er.User.UserId}...{Environment.NewLine}For event with Title: {er.Event.Title} wich will Start at: {er.Event.StartDate}{Environment.NewLine}{Environment.NewLine}");

                ++releasedReminders;
                _logger.LogInformation($"{releasedReminders} reminders of {totalRemindersToSend} have been sent{Environment.NewLine}{Environment.NewLine}");

                er.EventSuscription.EventReminderSent = true;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, $"Error occurred sending event reminder to:{Environment.NewLine}User: {er.User.UserName} with Id: {er.User.UserId}{Environment.NewLine}For event with Title: {er.Event.Title} which will Start at: {er.Event.StartDate}.");
            }
        }

        string format = $"{releasedReminders} reminders have been sent";

        _logger.LogInformation(format);

        _logger.LogInformation("Searching for other events to send reminders...");
    }
}
