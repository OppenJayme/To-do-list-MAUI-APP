using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using TodoListApp1.Models;

namespace TodoListApp1.Page
{
    public partial class AddTaskPage : ContentPage
    {
        readonly ObservableCollection<ToDoItem> _sharedItems;
        static readonly HttpClient _http = new() { BaseAddress = new Uri("https://todo-list.dcism.org") };

        // <-- new ctor
        public AddTaskPage(ObservableCollection<ToDoItem> items)
        {
            InitializeComponent();
            _sharedItems = items;
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
        }

        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            var title = TitleEntry.Text?.Trim();
            var description = DescriptionEditor.Text?.Trim();
            var userId = Preferences.Get("user_id", 0);

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
            {
                await DisplayAlert("Error", "Both fields are required.", "OK");
                return;
            }

            var payload = new
            {
                item_name = title,
                item_description = description,
                user_id = userId
            };
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await _http.PostAsync("/addItem_action.php", content);
                var body = await response.Content.ReadAsStringAsync();
                var apiResult = JsonSerializer.Deserialize<ApiResponse>(body)
                                     ?? throw new JsonException("Empty response");

                if (response.IsSuccessStatusCode && apiResult.Status == 200)
                {
                    await DisplayAlert("Success", apiResult.Message, "OK");

                    // **add into the shared collection**
                    _sharedItems.Add(new ToDoItem
                    {
                        Title = title,
                        IsCompleted = false
                    });

                    // pop back to TaskPage (its CollectionView is bound to the same Items)
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", apiResult.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not add task: {ex.Message}", "OK");
            }
        }
    }
}
