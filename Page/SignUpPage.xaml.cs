namespace TodoListApp1.Page
{
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage()
        {
            InitializeComponent();
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
        }

        private async void GoToSignIn(object sender, EventArgs e)
        {
            // Go back to LoginPage
            await Shell.Current.GoToAsync("//LogInPage");
        }

        private async void OnLabelTapped(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//LogInPage");
        }
    }
}
