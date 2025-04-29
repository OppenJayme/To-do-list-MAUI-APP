using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private readonly HttpClient _httpClient;

        public TaskPage()
        {
            InitializeComponent();

            // Set up HttpClient (change BaseAddress as needed)
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://todo-list.dcism.org")
            };

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
                // TODO: replace "/tasks.php" with your real endpoint
                var jsonString = await _httpClient.GetStringAsync("/tasks.php");

                var list = JsonSerializer.Deserialize<List<ToDoItem>>(jsonString);
                if (list != null)
                    Items = new ObservableCollection<ToDoItem>(list);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not load tasks: {ex.Message}", "OK");
            }
        }

        // 6) Add button handler
        private void OnAddClicked(object sender, EventArgs e)
        {
            Items.Add(new ToDoItem
            {
                Title = $"title {Items.Count + 1}",
                IsCompleted = false
            });
        }

        // 7) Delete command
        private void DeleteItem(ToDoItem item)
        {
            if (item != null && Items.Contains(item))
                Items.Remove(item);
        }

        // 8) Complete command
        private void CompleteItem(ToDoItem item)
        {
            if (item == null) return;
            item.IsCompleted = true;
            // e.g. navigate or re-load a "completed" page
            Shell.Current.GoToAsync("CompletedTaskPage");
        }

        // 9) Tap-on-task handler (if you have a Label with a TapGestureRecognizer)
        private async void OnTaskTapped(object sender, TappedEventArgs e)
        {
            if (sender is Label lbl && lbl.BindingContext is ToDoItem task)
            {
                await Navigation.PushAsync(new EditCompletedTask(task));
            }
        }
    }
}
