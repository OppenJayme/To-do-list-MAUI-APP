using System.Text.Json.Serialization;

public class ToDoItem
{
    [JsonPropertyName("item_id")]
    public int ItemId { get; set; }

    [JsonPropertyName("item_name")]
    public string Item_Name { get; set; }

    [JsonPropertyName("item_description")]
    public string Item_Description { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("timemodified")]
    public string TimeModified { get; set; }

    public string Title => Item_Name;
    public bool IsCompleted => Status == "inactive";
    
    public string DescriptionPreview => Item_Description.Length > 30 ? Item_Description.Substring(0, 30) + "..." : Item_Description;
}