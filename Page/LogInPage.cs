using System.ComponentModel;
using System.Text.Json;
using TodoListApp1.Models;

namespace TodoListApp1.Page
{
    public partial class LogInPage : ContentPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(nameof(Email)); }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://todo-list.dcism.org") };

        public LogInPage()
        {
            InitializeComponent();
            BindingContext = this;
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void OnLogInClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await DisplayAlert("Error", "Email and Password are required.", "OK");
                return;
            }

            try
            {
                var url = $"/signin_action.php?email={Uri.EscapeDataString(Email)}&password={Uri.EscapeDataString(Password)}";
                var response = await _httpClient.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Error", $"Server returned {(int)response.StatusCode}:\n{body}", "OK");
                    return;
                }

                SignInResponse result;
                try
                {
                    result = JsonSerializer.Deserialize<SignInResponse>(body)
                             ?? throw new JsonException("Empty response");
                }
                catch (JsonException)
                {
                    await DisplayAlert("Error", $"Invalid JSON response:\n{body}", "OK");
                    return;
                }

                if (result.Status == 200)
                {
                    await DisplayAlert("Success", result.Message ?? "Logged in.", "OK");

                    // Save user ID
                    Preferences.Set("user_id", result.Data.Id);

                    Application.Current.MainPage = new AppShell("//TaskPage");
                }
                else
                {
                    await DisplayAlert("Error", result.Message ?? "Login failed.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void SignUpTapped(object sender, EventArgs e)
            => await Navigation.PushAsync(new SignUpPage());
    }
}
