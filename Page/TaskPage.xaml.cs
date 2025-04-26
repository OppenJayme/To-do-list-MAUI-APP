using System.Collections.ObjectModel;
using System.Text.Json;
using TodoListApp1.Models;

namespace TodoListApp1.Page;

public partial class TaskPage : ContentPage
{
    public ObservableCollection<ToDoItem> Items { get; set; }
    public Command<ToDoItem> DeleteCommand { get; }
    public Command<ToDoItem> CompleteCommand { get; }

    public TaskPage()
    {
        InitializeComponent();
        Items = new ObservableCollection<ToDoItem>();
        DeleteCommand = new Command<ToDoItem>(DeleteItem);
        CompleteCommand = new Command<ToDoItem>(CompleteItem);
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        int userId = Preferences.Get("user_id", 0);
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

            var result = JsonSerializer.Deserialize<ToDoResponse>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result?.Status == 200)
            {
                Items.Clear();
                foreach (var item in result.Data.Values)
                {
                    Items.Add(item);
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

    private void DeleteItem(ToDoItem item)
    {
        if (Items.Contains(item))
            Items.Remove(item);
    }
    
    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is ToDoItem item)
        {
            Console.WriteLine("Navigating to EditTaskPage with Title: " + item.Title);
            await Navigation.PushAsync(new EditTaskPage(item));
        }
    }

    private void CompleteItem(ToDoItem item)
    {
        item.Status = "inactive"; // Local UI update; not persisted unless you implement it
        // Navigate or move item elsewhere if needed
    }

    private async void OnTaskTapped(object? sender, TappedEventArgs e)
    {
        if ((sender as Label)?.BindingContext is ToDoItem task)
        {
            await Navigation.PushAsync(new EditTaskPage(task));
        }
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddTaskPage());
    }
}
