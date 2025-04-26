namespace TodoListApp1.Models
{
    public class ToDoItem
    {
        public int ItemId { get; set; }
        public string Item_Name { get; set; }
        public string Item_Description { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public string TimeModified { get; set; }

        // Convenience property to display in the UI
        public string Title => Item_Name;
        public string Content => Item_Description;

        // Optional property for UI use only
        public bool IsCompleted => Status == "inactive";
    }
}