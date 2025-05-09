namespace TodoListApp1.Page
{
    public partial class ProfilePage : ContentPage
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        
        public ProfilePage()
        {
            InitializeComponent();

            FullName = $"{Preferences.Get("user_fname", string.Empty)} {Preferences.Get("user_lname", string.Empty)}";
            Email = Preferences.Get("user_email", string.Empty);

            // Set the binding context to this page
            BindingContext = this;
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