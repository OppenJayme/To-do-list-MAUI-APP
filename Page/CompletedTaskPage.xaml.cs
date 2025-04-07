using System.Collections.ObjectModel;
using TodoListApp1.Models;

namespace TodoListApp1.Page;

public partial class CompletedTaskPage : ContentPage
{
    public ObservableCollection<ToDoItem> CompletedItems { get; set; }
    public Command<ToDoItem> DeleteCompletedCommand { get; }
    public Command<ToDoItem> MoveBackToPendingCommand { get; }

    public CompletedTaskPage()
    {
        InitializeComponent();
        CompletedItems = new ObservableCollection<ToDoItem>
        {
            new ToDoItem { Title = "Completed task 1", IsCompleted = true }
        };

        DeleteCompletedCommand = new Command<ToDoItem>(DeleteCompletedItem);

        BindingContext = this;
    }

    private void DeleteCompletedItem(ToDoItem item)
    {
        if (CompletedItems.Contains(item))
            CompletedItems.Remove(item);
    }

}
