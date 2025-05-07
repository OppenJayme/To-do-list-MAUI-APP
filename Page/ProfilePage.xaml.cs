namespace TodoListApp1.Page
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        private async void OnSignOutClicked(object? sender, EventArgs e)
        {
            // Display a confirmation alert
            bool isConfirmed = await DisplayAlert(
                "Sign Out",
                "Are you sure you want to log out?",
                "Yes",   // Confirm button
                "No"     // Cancel button
            );

            // If user confirms, log out
            if (isConfirmed)
            {
                Application.Current.Windows.FirstOrDefault().Page = new NavigationPage(new LogInPage());
            }
        }
    }
}