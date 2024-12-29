
using Ordo.Models;

namespace ordo.Models
{
    public class ScheduleResponse
    {
        public List<EventData> ScheduledEvents { get; set; } = new List<EventData>();
        public List<ProblematicTask> ProblematicTasks { get; set; } = new List<ProblematicTask>();
        public string Notes { get; set; } = string.Empty;
    }

    public class ProblematicTask
    {
        public string Project { get; set; } = string.Empty;
        public string Task { get; set; } = string.Empty;
    }

}
