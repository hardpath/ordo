
//using Microsoft.Graph.Models;
using System.Text.Json.Serialization;

namespace Ordo.Models
{
    public class TodoData
    {
        public List<TodoList> Lists { get; set; } = new();
        public List<TodoTask> Tasks { get; set; } = new();
    }

    public class TodoListsResponse
    {
        [JsonPropertyName("value")]
        public List<TodoList>? Value { get; set; }
    }

    public class TodoTasksResponse
    {
        [JsonPropertyName("value")]
        public List<TodoTask>? Value { get; set; }
    }
    
    public class TodoList
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = string.Empty;
    }

    public class TodoTask
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("dueDateTime")]
        public DateTimeZone? DueDateTime { get; set; }

        public string ListId { get; set; } = string.Empty;
    }

    public class DateTimeZone
    {
        [JsonPropertyName("dateTime")]
        public string DateTime { get; set; } = string.Empty;

        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; } = string.Empty;
    }
}
