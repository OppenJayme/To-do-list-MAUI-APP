using System.Collections.ObjectModel;
using System.Text.Json;
using TodoListApp1.Models;
using System.Text;


namespace TodoListApp1.Page
{
    public partial class CompletedTaskPage : ContentPage
    {
        public ObservableCollection<ToDoItem> CompletedItems { get; set; }

        public CompletedTaskPage()
        {
            InitializeComponent();
            CompletedItems = new ObservableCollection<ToDoItem>();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            int userId = Preferences.Get("user_id", 0);
            Console.WriteLine($"Current User ID: {userId}");
            await LoadCompletedTasksAsync(userId);
        }

        private async Task LoadCompletedTasksAsync(int userId)
        {
            try
            {
                using var client = new HttpClient();
                var url = $"https://todo-list.dcism.org/getItems_action.php?status=inactive&user_id={userId}";
                var response = await client.GetStringAsync(url);

                var result = JsonSerializer.Deserialize<ToDoResponse>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Status == 200)
                {
                    CompletedItems.Clear();

                    if (result.Data != null)
                    {
                        foreach (var item in result.Data.Values)
                        {
                            CompletedItems.Add(item);
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Failed to load completed tasks", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception", ex.Message, "OK");
            }
            
        }
        private async void OnUndoClicked(object sender, EventArgs e)
        {
            var button = sender as ImageButton;
            if (button?.BindingContext is not ToDoItem task)
                return;

            try
            {
                using var client = new HttpClient();

                var requestBody = new
                {
                    status = "active",
                    item_id = task.ItemId
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                Console.WriteLine(json);
                var response = await client.PostAsync("https://todo-list.dcism.org/statusItem_action.php", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Undo Task - Server response: " + responseBody);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<GenericResponse>(responseBody);
                    if (result?.Status == 200)
                    {
                        await DisplayAlert("Marked as Incomplete", result.Message, "OK");

                        // Optionally remove task from UI or reload
                        var itemToRemove = CompletedItems.FirstOrDefault(x => x.ItemId == task.ItemId);
                        if (itemToRemove != null)
                            CompletedItems.Remove(itemToRemove);
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
        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var button = sender as ImageButton;
            if (button?.BindingContext is not ToDoItem task)
                return;

            var confirm = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this task?", "Yes", "No");
            if (!confirm)
                return;

            try
            {
                using var client = new HttpClient();

                // Send as application/x-www-form-urlencoded
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("item_id", task.ItemId.ToString())
                });

                var response = await client.PostAsync("https://todo-list.dcism.org/deleteItem_action.php", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Delete Task - Server response: " + responseBody);
                Console.WriteLine($"Deleted Task ID: {task.ItemId}");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<GenericResponse>(responseBody);
                    if (result?.Status == 200)
                    {
                        await DisplayAlert("Task Deleted", result.Message, "OK");

                        var itemToRemove = CompletedItems.FirstOrDefault(x => x.ItemId == task.ItemId);
                        if (itemToRemove != null)
                            CompletedItems.Remove(itemToRemove);
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
    
}