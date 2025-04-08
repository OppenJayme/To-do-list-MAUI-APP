namespace TodoListApp1.Page;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
	}

	private void OnSignOutClicked(object? sender, EventArgs e)
	{
		Navigation.PushAsync(new LogInPage());
	}
}