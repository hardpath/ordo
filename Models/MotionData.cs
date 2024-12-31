
using System.Text.Json.Serialization;

namespace Ordo.Models
{
    public class MotionData
    {
        public List<MotionWorkspace> Workspaces { get; set; } = new();
        public List<MotionProject> Projects { get; set; } = new();
        public List<MotionTask> Tasks { get; set; } = new();
    }

    public class MotionErrorResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }
    }

    public class MotionWorkspacesResponse
    {
        [JsonPropertyName("workspaces")]
        public List<MotionWorkspace> Workspaces { get; set; } = new();

        [JsonPropertyName("cursor")]
        public string? Cursor { get; set; }
    }

    public class MotionProjectsResponse
    {
        [JsonPropertyName("projects")]
        public List<MotionProject> Projects { get; set; } = new();

        [JsonPropertyName("cursor")]
        public string? Cursor { get; set; }
    }

    public class MotionTasksResponse
    {
        [JsonPropertyName("tasks")]
        public List<MotionTask> Tasks { get; set; } = new();

        [JsonPropertyName("cursor")]
        public string? Cursor { get; set; }
    }

    public class MotionWorkspace
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class MotionProject
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("workspaceId")]
        public string WorkspaceId { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }

    public class MotionTask
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("dueDate")]
        public DateTime DueDate { get; set; } = DateTime.MinValue;

        [JsonPropertyName("project")]
        public MotionProject Project { get; set; } = new MotionProject();
    }

}
