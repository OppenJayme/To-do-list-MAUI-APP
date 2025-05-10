using System.Text.Json;
using System.Text;
using TodoListApp1.Models;

namespace TodoListApp1.Page;

public partial class ViewTaskPage : ContentPage
{
    private readonly ToDoItem _task;
    private readonly string _status;

    public ViewTaskPage(ToDoItem task, string status)
    {
        InitializeComponent();

        _task = task;
        _status = status;

        TitleLabel.Text = task.Title;
        DescriptionLabel.Text = task.Item_Description;

        CompleteButton.IsVisible = _status == "active";
        IncompleteButton.IsVisible = _status == "inactive";

        CancelEditButton.IsVisible = false; // Initially hidden
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        TitleLabel.Text = _task.Title;
        DescriptionLabel.Text = _task.Item_Description;

        TitleEntry.Placeholder = _task.Title;
        DescriptionEntry.Placeholder = _task.Item_Description;

        TitleEntry.Text = "";
        DescriptionEntry.Text = "";
    }

    private void OnEditClicked(object sender, EventArgs e)
    {
        if (EditButton.Text == "Edit Task Details")
        {
            TitleEntry.Placeholder = _task.Item_Name;
            DescriptionEntry.Placeholder = _task.Item_Description;

            TitleEntry.Text = "";
            DescriptionEntry.Text = "";

            TitleLabel.IsVisible = false;
            TitleEntry.IsVisible = true;

            DescriptionLabel.IsVisible = false;
            DescriptionEntry.IsVisible = true;

            CancelEditButton.IsVisible = true;
            
            CompleteButton.IsEnabled = false;
            CompleteButton.BackgroundColor = Colors.LightGray;
            IncompleteButton.IsEnabled = false;
            IncompleteButton.BackgroundColor = Colors.LightGray;
            DeleteButton.IsEnabled = false;
            DeleteButton.BackgroundColor = Colors.LightGray;

            EditButton.Text = "Save Changes";
        }
        else
        {
            SaveTaskDetails();
        }
    }

    private void OnCancelEditClicked(object sender, EventArgs e)
    {
        TitleEntry.IsVisible = false;
        DescriptionEntry.IsVisible = false;
        TitleLabel.IsVisible = true;
        DescriptionLabel.IsVisible = true;
        
        TitleEntry.Text = "";
        DescriptionEntry.Text = "";
        
        EditButton.Text = "Edit Task Details";
        CancelEditButton.IsVisible = false;
        
        CompleteButton.IsEnabled = true;
        CompleteButton.BackgroundColor = Color.FromArgb("#4ECDC4");
        IncompleteButton.IsEnabled = true;
        IncompleteButton.BackgroundColor = Color.FromArgb("#FF9F00");
        DeleteButton.IsEnabled = true;
        DeleteButton.BackgroundColor = Color.FromArgb("#FF6B6B");
    }

    private async void SaveTaskDetails()
    {
        string newTitle = TitleEntry.Text?.Trim();
        string newDescription = DescriptionEntry.Text?.Trim();

        // Prevent empty update
        if (string.IsNullOrEmpty(newTitle) && string.IsNullOrEmpty(newDescription))
        {
            await DisplayAlert("Validation Error", "Please fill in at least one field before saving.", "OK");
            return;
        }

        // Use current values if input is empty
        if (string.IsNullOrEmpty(newTitle)) newTitle = _task.Item_Name;
        if (string.IsNullOrEmpty(newDescription)) newDescription = _task.Item_Description;

        // Prevent no-change update
        if (newTitle == _task.Item_Name && newDescription == _task.Item_Description)
        {
            await DisplayAlert("Validation Error", "You haven't changed one of the fields", "OK");
            return;
        }

        try
        {
            using var client = new HttpClient();
            var url = "https://todo-list.dcism.org/editItem_action.php";

            var data = new
            {
                item_name = newTitle,
                item_description = newDescription,
                item_id = _task.ItemId
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Edit Task - Response: " + responseBody);

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<GenericResponse>(responseBody);
                if (result?.Status == 200)
                {
                    await DisplayAlert("Success", result.Message, "OK");

                    // Update internal task model with the new values
                    _task.Item_Name = newTitle;
                    _task.Item_Description = newDescription;

                    // Update UI
                    TitleLabel.Text = newTitle;
                    DescriptionLabel.Text = newDescription;

                    TitleEntry.Placeholder = newTitle;
                    DescriptionEntry.Placeholder = newDescription;

                    TitleEntry.Text = "";
                    DescriptionEntry.Text = "";

                    TitleEntry.IsVisible = false;
                    DescriptionEntry.IsVisible = false;
                    TitleLabel.IsVisible = true;
                    DescriptionLabel.IsVisible = true;

                    EditButton.Text = "Edit Task Details";
                    CancelEditButton.IsVisible = false;

                    CompleteButton.IsEnabled = true;
                    CompleteButton.BackgroundColor = Color.FromArgb("#4ECDC4");
                    IncompleteButton.IsEnabled = true;
                    IncompleteButton.BackgroundColor = Color.FromArgb("#FF9F00");
                    DeleteButton.IsEnabled = true;
                    DeleteButton.BackgroundColor = Color.FromArgb("#FF6B6B");
                }
                else
                {
                    await DisplayAlert("Error", result?.Message ?? "Failed to update task", "OK");
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


    private async void OnCompleteClicked(object sender, EventArgs e)
    {
        var confirm = await DisplayAlert("Mark as Complete", $"Mark {_task.Title} as complete?", "Yes", "No");
        if (!confirm) return;

        try
        {
            using var client = new HttpClient();
            var requestBody = new { status = "inactive", item_id = _task.ItemId };
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://todo-list.dcism.org/statusItem_action.php", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Complete Task - Server response: " + responseBody);

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<GenericResponse>(responseBody);
                if (result?.Status == 200)
                {
                    await DisplayAlert("Marked as Complete", result.Message, "OK");
                    CompleteButton.IsVisible = false;
                    IncompleteButton.IsVisible = true;
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

    private async void OnIncompleteClicked(object sender, EventArgs e)
    {
        var confirm = await DisplayAlert("Mark as Incomplete", $"Mark {_task.Title} as incomplete?", "Yes", "No");
        if (!confirm) return;

        try
        {
            using var client = new HttpClient();
            var requestBody = new { status = "active", item_id = _task.ItemId };
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://todo-list.dcism.org/statusItem_action.php", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Incomplete Task - Server response: " + responseBody);

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<GenericResponse>(responseBody);
                if (result?.Status == 200)
                {
                    await DisplayAlert("Marked as Incomplete", result.Message, "OK");
                    CompleteButton.IsVisible = true;
                    IncompleteButton.IsVisible = false;
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
        var confirm = await DisplayAlert("Confirm Delete", $"Delete {_task.Title}?", "Yes", "No");
        if (!confirm) return;

        try
        {
            using var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("item_id", _task.ItemId.ToString())
            });

            var response = await client.PostAsync("https://todo-list.dcism.org/deleteItem_action.php", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Delete Task - Server response: " + responseBody);

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<GenericResponse>(responseBody);
                if (result?.Status == 200)
                {
                    await DisplayAlert("Task Deleted", result.Message, "OK");
                    await Navigation.PopAsync();
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
