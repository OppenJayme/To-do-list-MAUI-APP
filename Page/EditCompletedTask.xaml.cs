using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoListApp1.Models;

namespace TodoListApp1.Page;

public partial class EditCompletedTask : ContentPage
{
    
    public ToDoItem Task { get; set; }

    public EditCompletedTask(ToDoItem task)
    {
        InitializeComponent();
        Task = task; // Store the task object for later use
        BindingContext = this; // Bind the task to the page's UI
    }

    private void OnUpdateClicked(object? sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void OnMarkUnfinishedClicked(object? sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void OnDeleteClicked(object? sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}

