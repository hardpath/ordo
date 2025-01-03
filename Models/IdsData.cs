using System.Text.Json.Serialization;

namespace Ordo.Models
{
    internal class IdsData
    {
        [JsonPropertyName("todoid")]
        public string ToDoId { get; set; } = string.Empty;

        [JsonPropertyName("motionid")]
        public string MotionId{ get; set; } = string.Empty;
    }
}
