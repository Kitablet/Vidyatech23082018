using System;

namespace Kitablet.ViewModels
{
    public class UserEvent
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsView { get; set; }
    }
    public class UserEventResult
    {
        public UserEventStatus Status { get; set; }
        public UserEvent MyEvents { get; set; }
    }
}
