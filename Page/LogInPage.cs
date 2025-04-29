using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Maui.Controls;
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
                // Build the GET URL
                var url = $"/signin_action.php?email={Uri.EscapeDataString(Email)}&password={Uri.EscapeDataString(Password)}";

                // Call the API
                var response = await _httpClient.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();

                // If not 2xx, show raw body for debugging
                if (!response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Error",
                        $"Server returned {(int)response.StatusCode}:\n{body}", "OK");
                    return;
                }

                // Try to parse JSON
                SignInResponse result;
                try
                {
                    result = JsonSerializer.Deserialize<SignInResponse>(body)
                             ?? throw new JsonException("Empty response");
                }
                catch (JsonException)
                {
                    await DisplayAlert("Error",
                        $"Invalid JSON response:\n{body}", "OK");
                    return;
                }

                // Check API status field
                if (result.Status == 200)
                {
                    await DisplayAlert("Success", result.Message ?? "Logged in.", "OK");

                    // Swap into your Shell with tabs
                   
                        Application.Current.MainPage = new AppShell();
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
