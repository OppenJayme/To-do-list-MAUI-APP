using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using TodoListApp1.Models;

namespace TodoListApp1.Page
{
    public partial class AddTaskPage : ContentPage
    {
        static readonly HttpClient _http = new() { BaseAddress = new Uri("https://todo-list.dcism.org") };

        public AddTaskPage()
        {
            InitializeComponent();
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
                user_id = userId,
                status = "active"
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

                Console.WriteLine("Task Response Body: " + body);

                var apiResult = JsonSerializer.Deserialize<ApiResponse>(body)
                                ?? throw new JsonException("Empty response");

                if (response.IsSuccessStatusCode && apiResult.Status == 200)
                {
                    await DisplayAlert("Success", apiResult.Message, "OK");

                    // ✅ No need to add manually — TaskPage will refresh itself
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
