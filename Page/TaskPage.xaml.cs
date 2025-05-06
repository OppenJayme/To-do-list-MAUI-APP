using System.Collections.ObjectModel;
using System.Text.Json;
using TodoListApp1.Models;
using System.Text;

namespace TodoListApp1.Page;

public partial class TaskPage : ContentPage
{
    public ObservableCollection<ToDoItem> Items { get; set; }
    
    public TaskPage()
    {
        InitializeComponent();
        Items = new ObservableCollection<ToDoItem>();
        BindingContext = this;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        int userId = Preferences.Get("user_id", 0);
        Console.WriteLine($"Current User ID: {userId}");
        System.Diagnostics.Debug.WriteLine($"[TaskPage] Current user ID: {userId}"); // Logs to output
        await LoadTasksAsync("active", userId);
    }
    
    private async Task LoadTasksAsync(string status, int userId)
    {
        try
        {
            using var client = new HttpClient();
            var url = $"https://todo-list.dcism.org/getItems_action.php?status={status}&user_id={userId}";
            var response = await client.GetStringAsync(url);

            // Deserialize the response
            var result = JsonSerializer.Deserialize<ToDoResponse>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            
            if (result?.Status == 200)
            {
                Items.Clear();

                // Loop through the tasks and add them to the ObservableCollection
                if (result.Data != null)
                {
                    Console.WriteLine("Raw JSON from server: " + response);

                    foreach (var item in result.Data.Values)
                    {
                        Console.WriteLine($"Deserialized item: ID={item.ItemId}, Name={item.Item_Name}");   
                        // Add each task (ToDoItem) to the Items collection
                        Items.Add(item);
                    }
                }
            }
            else
            {
                await DisplayAlert("Error", "Failed to load tasks", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Exception", ex.Message, "OK");
        }
    }
    
    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddTaskPage());
    }
    
    private async void OnCheckTapped(object sender, EventArgs e)
    {
        var label = sender as Label;
        if (label?.BindingContext is not ToDoItem task)
            return;

        try
        {
            using var client = new HttpClient();

            var requestBody = new
            {
                status = "inactive", 
                item_id = task.ItemId
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            Console.WriteLine(json);
            var response = await client.PostAsync("https://todo-list.dcism.org/statusItem_action.php", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Complete Task - Server response: " + responseBody);

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<GenericResponse>(responseBody);
                if (result?.Status == 200)
                {
                    await DisplayAlert("Marked as Complete", result.Message, "OK");

                    // Optionally remove task from UI or reload
                    var itemToRemove = Items.FirstOrDefault(x => x.ItemId == task.ItemId);
                    if (itemToRemove != null)
                        Items.Remove(itemToRemove);
                }
                else
                {
                    await DisplayAlert("Failed", result?.Message ?? "Unknown error", "OK");
                }
            }
            else
            {
                await DisplayAlert("HTTP Error", $"Status {(int)response.StatusCode}: {responseBody}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Exception", ex.Message, "OK");
        }
    }
}
