using System.Text;
using System.Text.Json;
using TodoListApp1.Models;

namespace TodoListApp1.Page
{
    public partial class EditTaskPage : ContentPage
    {
        public ToDoItem Task { get; set; }
        private readonly HttpClient _httpClient;

        public bool IsEditable => !Task.IsCompleted;
        public string StatusButtonText => Task.IsCompleted ? "Mark Unfinished" : "Mark Finished";
        public string ItemName => Task.Item_Name;
        public string ItemDescription => Task.Item_Description;

        public EditTaskPage(ToDoItem task)
        {
            InitializeComponent();
            
            Task = task ?? throw new ArgumentNullException(nameof(task));
            _httpClient = new HttpClient();
            BindingContext = Task;
        }

        private async void OnUpdateClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Task.Item_Name) || string.IsNullOrWhiteSpace(Task.Item_Description))
            {
                await DisplayAlert("Error", "Both title and description are required.", "OK");
                return;
            }

            var updatedTaskData = new
            {
                item_id = Task.ItemId,
                item_name = Task.Item_Name,
                item_description = Task.Item_Description,
                user_id = Task.UserId,
                status = Task.Status
            };

            var jsonData = JsonSerializer.Serialize(updatedTaskData);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PutAsync("https://todo-list.dcism.org/editItem_action.php", content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<EditTaskResponse>(responseString);
                    await DisplayAlert("Success", result?.Message ?? "Item updated successfully.", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    var error = JsonSerializer.Deserialize<EditTaskResponse>(responseString);
                    await DisplayAlert("Error", error?.Message ?? "Failed to update item.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnStatusToggleClicked(object sender, EventArgs e)
        {
            Task.Status = Task.IsCompleted ? "active" : "inactive";

            // Update UI
            OnPropertyChanged(nameof(StatusButtonText));
            OnPropertyChanged(nameof(IsEditable));

            await DisplayAlert("Status Changed", $"Task marked as {(Task.IsCompleted ? "pending" : "completed")}.", "OK");
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            // Implement delete logic here if needed
            await Navigation.PopAsync();
        }
    }
}
