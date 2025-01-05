using System.Text.Json.Serialization;

namespace Ordo.Models
{
    public class IdsData
    {
        public List<IdPair> Lists { get; set; } = new List<IdPair>();
        public List<IdPair> Tasks { get; set; } = new List<IdPair>();

        public bool ListExistsInTodo(string id)
        {
            foreach (var pair in Lists) {
                if (pair.ToDoId == id) {
                    return true;
                }
            }
            return false;
        }

        public bool ListExistsInMotion(string id)
        {
            foreach (var pair in Lists) {
                if (pair.MotionId == id) {
                    return true;
                }
            }
            return false;
        }

        public bool TaskExistsInTodo(string id)
        {
            foreach (var pair in Tasks) {
                if (pair.ToDoId == id) {
                    return true;
                }
            }
            return false;
        }

        public bool TaskExistsInMotion(string id)
        {
            foreach (var pair in Tasks) {
                if (pair.MotionId == id) {
                    return true;
                }
            }
            return false;
        }

        public string GetProjectID(string listId)
        {
            foreach (var list in Lists) {
                if (list.ToDoId == listId) return list.MotionId;
            }

            return string.Empty;
        }

        public string GetTaskID(string taskId)
        {
            foreach (var task in Tasks) {
                if (task.ToDoId == taskId) return task.MotionId;
            }

            return string.Empty;
        }

        public bool DeleteTaskPair(string? toDoId = null, string? motionId = null)
        {
            if (toDoId == null && motionId == null) {
                return true; // Invalid arguments
            }

            IdPair? taskToRemove = null;

            // Iterate through the Tasks list to find the first matching pair
            foreach (var task in Tasks) {
                bool matchesToDoId = false;
                if (toDoId == null)
                    matchesToDoId = true;
                else if (task.ToDoId == toDoId)
                    matchesToDoId = true;

                bool matchesMotionId = false;
                if (motionId == null)
                    matchesMotionId = true;
                else if (task.MotionId == motionId)
                    matchesMotionId = true;

                if (matchesToDoId && matchesMotionId) {
                    taskToRemove = task;
                    break;
                }
            }

            // Remove the matching task if found
            if (taskToRemove != null) {
                Tasks.Remove(taskToRemove);
                return false;
            }

            return true; // Pair not found
        }
    }

    public class IdPair
    {
        [JsonPropertyName("todoid")]
        public string ToDoId { get; set; } = string.Empty;

        [JsonPropertyName("motionid")]
        public string MotionId { get; set; } = string.Empty;
    }
}
