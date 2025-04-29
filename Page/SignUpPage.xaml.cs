using System;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Maui.Controls;
using TodoListApp1.Models;

namespace TodoListApp1.Page
{
    public partial class SignUpPage : ContentPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(nameof(FirstName)); }
        }

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(nameof(LastName)); }
        }

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

        private string _confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(nameof(ConfirmPassword)); }
        }

        readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://todo-list.dcism.org") };

        public SignUpPage()
        {
            InitializeComponent();
            BindingContext = this;
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
        }

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                await DisplayAlert("Error", "All fields are required.", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            // Prepare JSON body
            var payload = new
            {
                first_name = FirstName,
                last_name = LastName,
                email = Email,
                password = Password,
                confirm_password = ConfirmPassword
            };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Call the API
                var response = await _httpClient.PostAsync("/signup_action.php", content);
                var responseString = await response.Content.ReadAsStringAsync();

                // Parse JSON (success or error)
                var apiResult = JsonSerializer.Deserialize<ApiResponse>(responseString);
                if (apiResult == null)
                {
                    await DisplayAlert("Error", "Unexpected server response.", "OK");
                    return;
                }

                if (apiResult.Status == 200)
                {
                    await DisplayAlert("Success", apiResult.Message, "OK");
                    // Go back to the login page on the Nav stack
                    if (Navigation?.NavigationStack?.Count > 1)
                        await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", apiResult.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnLabelTapped(object sender, EventArgs e)
        {
            // Simply pop back to login
            if (Navigation?.NavigationStack?.Count > 1)
                await Navigation.PopAsync();
        }
    }
}
