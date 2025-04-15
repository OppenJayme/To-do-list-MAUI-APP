namespace TodoListApp1.Page;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
	}
	
	private void OnSignOutClicked(object? sender, EventArgs e)
	{
		Application.Current.Windows.FirstOrDefault().Page = new NavigationPage(new LogInPage());
	}
}