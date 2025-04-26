using System.Net.Http;
using System.Text.Json;
using TodoListApp1.Models;
using System.Collections.ObjectModel;

namespace TodoListApp1.Page;

public partial class CompletedTaskPage : ContentPage
{
    private readonly HttpClient _httpClient = new();
    public ObservableCollection<ToDoItem> CompletedItems { get; set; } = new();

    public Command<ToDoItem> DeleteCompletedCommand { get; }

    public CompletedTaskPage()
    {
        InitializeComponent();
        BindingContext = this;

        DeleteCompletedCommand = new Command<ToDoItem>(async (item) => await DeleteCompletedItem(item));

        LoadCompletedTasks(); // ⬅️ Fetch tasks when page loads
    }

    private async void LoadCompletedTasks()
    {
        try
        {
            var response = await _httpClient.GetAsync("https://todo-list.dcism.org/items.php");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var allTasks = JsonSerializer.Deserialize<List<ToDoItem>>(json);

                var completedTasks = allTasks.Where(t => t.IsCompleted);

                CompletedItems.Clear();
                foreach (var task in completedTasks)
                {
                    CompletedItems.Add(task);
                }
            }
            else
            {
                await DisplayAlert("Error", "Failed to load tasks.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task DeleteCompletedItem(ToDoItem item)
    {
        var response = await _httpClient.DeleteAsync($"https://todo-list.dcism.org/deleteItem_action.php?item_id={item.ItemId}");
        if (response.IsSuccessStatusCode)
        {
            CompletedItems.Remove(item);
            await DisplayAlert("Success", "Task deleted.", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Failed to delete task.", "OK");
        }
    }

    private async void CompletedTaskTapped(object sender, TappedEventArgs e)
    {
        var label = sender as Label;
        var task = label?.BindingContext as ToDoItem;

        if (task != null)
            await Navigation.PushAsync(new EditTaskPage(task));
    }
}
