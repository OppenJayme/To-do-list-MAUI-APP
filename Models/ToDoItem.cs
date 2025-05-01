using System.Text.Json.Serialization;

namespace TodoListApp1.Models
{
    /// <summary>
    /// Represents a single to-do item.
    /// Adjust JsonPropertyName to match your API’s JSON fields.
    /// </summary>
    public class ToDoItem
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("isCompleted")]
        public bool IsCompleted { get; set; }
    }
}
