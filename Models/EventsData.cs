namespace Ordo.Models
{
    public class EventsData
    {
        public List<CalendarEvent> Events { get; set; } = new List<CalendarEvent>();

        public void Add(CalendarEvent calendarEvent)
        {
            if (calendarEvent != null && !EventExists(calendarEvent.Id)) {
                Events.Add(calendarEvent);
            }
        }

        private bool EventExists(string eventId)
        {
            foreach (var existingEvent in Events) {
                if (existingEvent.Id == eventId) {
                    return true;
                }
            }
            return false;
        }
    }

    public class CalendarEvent
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; } = DateTime.MinValue;
        public DateTime EndTime { get; set; } = DateTime.MinValue;
        public string? TaskLink { get; set; } = null;
        public bool IsOrdoCreated { get; set; } = false;
    }
}
