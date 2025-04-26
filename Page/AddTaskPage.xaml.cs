using System.Text;
using System.Text.Json;
using Microsoft.Maui.Controls;

namespace TodoListApp1.Page;

public partial class AddTaskPage : ContentPage
{
    public AddTaskPage()
    {
        InitializeComponent();
    }

    private async void OnAddTaskClicked(object sender, EventArgs e)
    {
        var title = TitleEntry.Text?.Trim();
        var description = DescriptionEditor.Text?.Trim();
        int userId = Preferences.Get("user_id", 0); // Default to 0 if not found
        
        System.Diagnostics.Debug.WriteLine($"[AddTaskPage] Current user ID: {userId}"); // Logs to output

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
        {
            await DisplayAlert("Error", "Both fields are required.", "OK");
            return;
        }

        var taskData = new
        {
            item_name = title,
            item_description = description,
            user_id = userId
        };

        var json = JsonSerializer.Serialize(taskData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        var response = await client.PostAsync("https://todo-list.dcism.org/addItem_action.php", content);

        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Success", "Task added successfully", "OK");
            await Navigation.PopAsync(); // Go back to task list
        }
        else
        {
            await DisplayAlert("Error", "Failed to add task", "OK");
        }
    }
}