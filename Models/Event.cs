using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inMemoryQueue.ccsnQueueTest.Models;

public class Event
{
    public ulong EventId { get; set; }
    public bool? Active { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? StartDate { get; set; }
}

public partial class EventSuscription
{
    public ulong Id { get; set; }
    public ulong EventId { get; set; }
    public int UserId { get; set; }
    public bool? Accepted { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool EventReminderSent { get; set; }
}

public class User
{
    public int UserId { get; set; }
    public string? UserName { get; set; }
}

public class EventReminder
{
    public User User { get; set; }
    public Event Event { get; set; }
    public EventSuscription EventSuscription { get; set; }
    // public bool Sent { get; set; }
}