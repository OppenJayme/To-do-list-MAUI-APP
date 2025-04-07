using System.Collections.ObjectModel;
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
        Items = new ObservableCollection<ToDoItem>
        {
            new ToDoItem { Title = "title 1", IsCompleted = false }
        };

        DeleteCommand = new Command<ToDoItem>(DeleteItem);
        CompleteCommand = new Command<ToDoItem>(CompleteItem);

        BindingContext = this;
    }

    private void OnAddClicked(object sender, EventArgs e)
    {
        Items.Add(new ToDoItem { Title = $"title {Items.Count + 1}", IsCompleted = false });
    }

    private void DeleteItem(ToDoItem item)
    {
        if (Items.Contains(item))
            Items.Remove(item);
    }

    private void CompleteItem(ToDoItem item)
    {
        item.IsCompleted = true;
        // Here you could move the item to another list (or use navigation to a separate page)
        // For simplicity, we'll assume items are moved to another page
        // Example: Navigating to CompletedTaskPage with the completed item list

        // Navigate to the CompletedTaskPage and pass the item
        Shell.Current.GoToAsync("CompletedTaskPage");
    }
    private async void OnTaskTapped(object? sender, TappedEventArgs tappedEventArgs)
    {
        // Get the task object from the sender's BindingContext
        var label = sender as Label;

        if (label?.BindingContext is ToDoItem task)
        {
            // Navigate to the EditCompletedTask page, passing the ToDoItem object
            await Application.Current.MainPage.Navigation.PushAsync(new EditCompletedTask(task));
        }
    }

}
