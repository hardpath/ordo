
namespace Ordo.Models
{
    internal class AppSettings
    {
        public bool? AbortTodoOverdue { get; set; } = null;
        public bool? AbortMotionZombie { get; set; } = null;
        public bool? Debug { get; set; } = null;
        public int? Refresh { get; set; } = null;
        public string? Log { get; set; } = null;

        public List<string> Validate()
        {
            var missingFields = new List<string>();

            if (AbortTodoOverdue == null)
                missingFields.Add(nameof(AbortTodoOverdue));

            if (AbortMotionZombie == null)
                missingFields.Add(nameof(AbortMotionZombie));

            if (Debug == null)
                missingFields.Add(nameof(Debug));

            if (Refresh == null)
                missingFields.Add(nameof(Refresh));

            if (Log == null)
                missingFields.Add(nameof(Log));

            return missingFields;
        }
    }
}
