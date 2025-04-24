using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.ComponentModel;

namespace TodoListApp1.Page
{
    public partial class SignUpPage : ContentPage, INotifyPropertyChanged
    {
         private readonly HttpClient _httpClient;

        public string FirstName { get; set; } = string.Empty; 
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;

        public SignUpPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri("https://todo-list.dcism.org") };
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
            BindingContext = this;
        }

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            // Validate user input
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || 
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

            // Prepare data for API
            var data = new
            {
                first_name = FirstName,
                last_name = LastName,
                email = Email,
                password = Password,
                confirm_password = ConfirmPassword
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Call the Sign Up API
                var response = await _httpClient.PostAsync("/signup_action.php", content);
                var responseString = await response.Content.ReadAsStringAsync();

                   if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ApiResponse>(responseString);
                    await DisplayAlert("Success", result?.Message ?? "Account created successfully.", "OK");
                    Application.Current.MainPage = new NavigationPage(new LogInPage());
                }
                else
                {
                    var error = JsonSerializer.Deserialize<ApiResponse>(responseString);
                    await DisplayAlert("Error", error?.Message ?? "An error occurred.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
        private async void OnLabelLogInTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LogInPage());
        }
    }

    public class ApiResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
    }
}