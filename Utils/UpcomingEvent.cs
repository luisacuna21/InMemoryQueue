using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inMemoryQueue.ccsnQueueTest.MockData;
using inMemoryQueue.ccsnQueueTest.Models;

namespace inMemoryQueue.ccsnQueueTest.Utils;

public class UpcomingEvent
{
    public List<Event> Events { get; set; }
    public List<User> Users { get; set; }
    public List<EventSuscription> EventSuscriptions { get; set; }

    public UpcomingEvent()
    {
        Events = new EventMockData().Events;
        Users = new UserMockData().Users;
        EventSuscriptions = new EventSuscriptionMockData().EventSuscriptions;
    }

    public IEnumerable<Event> GetUpcomingEvents()
    {
        var upcomingEvents = from e in Events
                             where e.StartDate > DateTime.Now && e.StartDate <= DateTime.Now.AddMinutes(5)
                             select e;

        return upcomingEvents;


        // var proxEvents = from es in EventSuscriptions
        //                  join e in Events on es.EventId equals e.EventId
        //                  join u in Users on es.UserId equals u.UserId
        //                  where e.EventId <= 10
        //                  orderby e.StartDate ascending
        //                  select new
        //                  {
        //                      EventId = e.EventId,
        //                      EventTitle = e.Title,
        //                      StartDate = e.StartDate,
        //                      UserId = u.UserId,
        //                      UserName = u.UserName
        //                  };
        // return proxEvents.ToList<object>();
    }

    public IEnumerable<EventReminder> GetEventSuscriptionReminders()
    {
        var eventSuscriptions = from es in EventSuscriptions
                                join e in Events on es.EventId equals e.EventId
                                join u in Users on es.UserId equals u.UserId
                                // where e.EventId <= 10
                                orderby e.StartDate ascending
                                select new EventReminder
                                {
                                    Event = e,
                                    User = u,
                                    EventSuscription = es
                                };
        return eventSuscriptions;
    }

    public IEnumerable<EventReminder> GetEventSuscriptionReminders(IEnumerable<Event> events)
    {
        var eventSuscriptions = from es in EventSuscriptions
                                join e in events on es.EventId equals e.EventId
                                join u in Users on es.UserId equals u.UserId
                                // where e.EventId <= 10
                                where es.EventReminderSent == false
                                orderby e.StartDate ascending
                                select new EventReminder
                                {
                                    Event = e,
                                    User = u,
                                    EventSuscription = es
                                };
        return eventSuscriptions;
        // var eventSuscriptions = from es in EventSuscriptions
        //                         join e in events on es.EventId equals e.EventId
        //                         join u in Users on es.UserId equals u.UserId
        //                         where e.EventId <= 10
        //                         orderby e.StartDate ascending
        //                         select new
        //                         {
        //                             EventId = e.EventId,
        //                             EventTitle = e.Title,
        //                             StartDate = e.StartDate,
        //                             UserId = u.UserId,
        //                             UserName = u.UserName
        //                         };
        // return eventSuscriptions;
    }
}
