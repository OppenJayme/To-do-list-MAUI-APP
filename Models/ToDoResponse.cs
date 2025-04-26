namespace TodoListApp1.Models
{
    public class ToDoResponse
    {
        public int Status { get; set; }
        public Dictionary<string, ToDoItem> Data { get; set; }
        public int Count { get; set; }
    }
}