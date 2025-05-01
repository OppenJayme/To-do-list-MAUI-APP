using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Maui.Controls;
using TodoListApp1.Models;

namespace TodoListApp1.Page
{
    public partial class TaskPage : ContentPage, INotifyPropertyChanged
    {
        // 1) INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // 2) Backing field + property for your ObservableCollection
        private ObservableCollection<ToDoItem> _items = new();
        public ObservableCollection<ToDoItem> Items
        {
            get => _items;
            set
            {
                if (_items == value) return;
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        // 3) Commands
        public Command<ToDoItem> DeleteCommand { get; }
        public Command<ToDoItem> CompleteCommand { get; }

        // 4) HTTP client for fetching tasks
        readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://todo-list.dcism.org") };

        public TaskPage() : ContentPage
        {
            InitializeComponent();

            // Wire up commands
            DeleteCommand = new Command<ToDoItem>(DeleteItem);
            CompleteCommand = new Command<ToDoItem>(CompleteItem);

            // Bind to this page so XAML `{Binding Items}` works
            BindingContext = this;
        }

        // 5) Fetch your tasks when the page appears
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var response = await _httpClient.GetAsync("/tasks.php");
                var body = await response.Content.ReadAsStringAsync();

                // 5a) 404 => no tasks
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Items.Clear();
                    return;
                }

                // 5b) other non-success => show error
                if (!response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Error",
                        $"Could not load tasks (status {(int)response.StatusCode}):\n{body}", "OK");
                    return;
                }

                // 5c) success => parse JSON
                var list = JsonSerializer.Deserialize<List<ToDoItem>>(body);
                Items.Clear();
                if (list != null)
                    foreach (var item in list)
                        Items.Add(item);
            }
            catch (HttpRequestException httpEx)
            {
                await DisplayAlert("Error", $"Network error: {httpEx.Message}", "OK");
            }
            catch (JsonException jsonEx)
            {
                await DisplayAlert("Error", $"Data error: {jsonEx.Message}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
            }
        }

        // 6) Add button handler
        private async void OnAddClicked(object sender, EventArgs e)
        {
            // pushes your AddTaskPage modally on top of the tabs
            await Navigation.PushAsync(new AddTaskPage(Items));
        }
        // 7) Delete command
        private void DeleteItem(ToDoItem item)
        {
            if (item != null && Items.Contains(item))
                Items.Remove(item);
        }

        // 8) Complete command
        private async void CompleteItem(ToDoItem item)
        {
            if (item == null)
                return;

            item.IsCompleted = true;
            // Navigate to the completed tab (by route), if desired
            await Shell.Current.GoToAsync("//CompletedPage");
        }

        // 9) Tap-on-task handler
        private async void OnTaskTapped(object sender, TappedEventArgs e)
        {
            if (sender is Label lbl && lbl.BindingContext is ToDoItem task)
            {
                // Navigate to an edit page, passing the task
                await Navigation.PushAsync(new EditCompletedTask(task));
            }
        }
    }
}
