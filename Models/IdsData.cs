using System.Text.Json.Serialization;

namespace Ordo.Models
{
    public class IdsData
    {
        public List<IdPair> Ids { get; set; } = new List<IdPair>();
    }

    public class IdPair
    {
        [JsonPropertyName("todoid")]
        public string ToDoId { get; set; } = string.Empty;

        [JsonPropertyName("motionid")]
        public string MotionId { get; set; } = string.Empty;
    }
}
