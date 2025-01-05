
//using Microsoft.Graph.Models;
using System.Text.Json.Serialization;

namespace Ordo.Models
{
    public class TodoData
    {
        public List<TodoList> Lists { get; set; } = new();
        public List<TodoTask> Tasks { get; set; } = new();
    
        public string GetListName(string listId)
        {
            foreach (var list in Lists) {
                if (list.Id == listId) return list.DisplayName;
            }

            return string.Empty;
        }

        public string GetListId(string taskID)
        {
            foreach (var task in Tasks) {
                if (task.Id == taskID) return task.ListId;
            }

            return string.Empty;
        }

        public bool TaskExists(string taskId)
        {
            foreach(var task in Tasks) {
                if (task.Id == taskId) return true;
            }
            return false;
        }

        public string GetTaskName(string taskId)
        {
            foreach (var task in Tasks) {
                if (task.Id == taskId) return task.Title;
            }
            return string.Empty;
        }
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

        public DateTime GetUtcTime()
        {
            try {
                if (!System.DateTime.TryParse(DateTime, out var localDateTime)) {
                    throw new FormatException("Invalid DateTime format.");
                }

                // Get the TimeZoneInfo object
                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);

                // Convert the local time to UTC
                var utcTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, timeZoneInfo);

                return utcTime;
            }
            catch (Exception ex) {
                throw new InvalidOperationException($"Failed to calculate UTC time: {ex.Message}");
            }
        }
    }

    // Error response
    public class GraphErrorResponse
    {
        [JsonPropertyName("error")]
        public GraphErrorDetail Error { get; set; } = new GraphErrorDetail();
    }

    public class GraphErrorDetail
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("innerError")]
        public GraphInnerErrorDetail InnerError { get; set; } = new GraphInnerErrorDetail();
    }

    public class GraphInnerErrorDetail
    {
        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;

        [JsonPropertyName("request-id")]
        public string RequestId { get; set; } = string.Empty; // Mapped to "request-id"

        [JsonPropertyName("client-request-id")]
        public string ClientRequestId { get; set; } = string.Empty; // Mapped to "client-request-id"
    }

}
