
using System.Text.Json.Serialization;

namespace Ordo.Models
{
    public class MotionData
    {
        public List<MotionWorkspace> Workspaces { get; set; } = new();
        public List<MotionProject> Projects { get; set; } = new();
        public List<MotionTask> Tasks { get; set; } = new();

        public string GetWorkspaceId(string workspaceName) {
            foreach (var workspace in Workspaces) {
                if (workspace.Name == workspaceName) return workspace.Id;
            }

            return string.Empty;
        }
    
        public bool TaskHasChanged(string id, string name, DateTime dueDate)
        {
            foreach (var task in Tasks) {
                if (task.Id == id) {
                    if (task.Name != name) return true;
                    if (task.DueDate.Date != dueDate.Date) return true;
                    return false;
                }
            }
            return false;
        }
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

    // RESPONSES
    public class MotionErrorResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }
    }

    public class MotionErrorResponse2
    {
        [JsonPropertyName("message")]
        public List<string> Message { get; set; } = new List<string>();

        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }
    }

    public class MotionWorkspacesResponse
    {
        [JsonPropertyName("workspaces")]
        public List<MotionWorkspace> Workspaces { get; set; } = new();

        //TODO 1-Check this (it doesnt' match the documentation)
        [JsonPropertyName("cursor")]
        public string? Cursor { get; set; }
    }

    public class MotionProjectsResponse
    {
        [JsonPropertyName("projects")]
        public List<MotionProject> Projects { get; set; } = new();

        //TODO 1-Check this (it doesnt' match the documentation)
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

}
