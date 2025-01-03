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
    }

    public class IdPair
    {
        [JsonPropertyName("todoid")]
        public string ToDoId { get; set; } = string.Empty;

        [JsonPropertyName("motionid")]
        public string MotionId { get; set; } = string.Empty;
    }
}
