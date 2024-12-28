using Microsoft.Graph.Models;
using System;

namespace Ordo.Models
{
    public class EventsData
    {
        public List<CalendarEvent> Events { get; set; } = new List<CalendarEvent>();

        public void Add(CalendarEvent cevent)
        {
            Events.Add(cevent);
        }
    }

    public class CalendarEvent
    {
        public string Id { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsOrdoCreated { get; set; } = false;
    }

    public class RawEvent
    {
        public string id { get; set; } = string.Empty;
        public string subject { get; set; } = string.Empty;
        public DateTimeZone? start { get; set; }
        public DateTimeZone? end { get; set; }
        public string type { get; set; } = string.Empty;
    }

    public class DateTimeZone
    {
        public string dateTime { get; set; } = string.Empty;
        public string timeZone { get; set; } = string.Empty;

        public DateTime ToUTCDateTime()
        {
            DateTime localDateTime = DateTime.Parse(dateTime);
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);

            return TimeZoneInfo.ConvertTimeToUtc(localDateTime, timeZoneInfo);
        }
    }
}
