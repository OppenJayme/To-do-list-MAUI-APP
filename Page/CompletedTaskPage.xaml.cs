using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using TodoListApp1.Models;

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
    }
}