using TodoListApp1.Models;

namespace TodoListApp1.Page;

public partial class LoadingPage : ContentPage
{
    public LoadingPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(2500);
        await Shell.Current.GoToAsync("//TaskPage");
    }
}