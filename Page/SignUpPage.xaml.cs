using Microsoft.Maui.Controls;

namespace TodoListApp1.Page
{
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        private async void GoToSignIn(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Page1());
        }

         

    }
}