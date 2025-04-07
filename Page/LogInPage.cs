using Microsoft.Maui.Controls;
using System.Threading.Tasks;


namespace TodoListApp1.Page
{
	public partial class LogInPage : ContentPage
	{
		public LogInPage()
		{
            InitializeComponent();
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
        }
        //private async void GoToSignUp(object sender, EventArgs e)
        //{
        //    await Navigation.PushAsync(new SignUpPage());
        //}

        private async void SignUpTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUpPage());
        }

        private async void OnLogInClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//TaskPage");
        }

    }

}