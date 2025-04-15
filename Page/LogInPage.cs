using Microsoft.Maui.Controls;
using System.Net.Http;
using System.Text.Json;

namespace TodoListApp1.Page
{
    public partial class LogInPage : ContentPage
    {
        private readonly HttpClient _httpClient;

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public LogInPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri("https://todo-list.dcism.org") };
            BindingContext = this; // Bind the page to this class
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
            NavigationPage.SetHasNavigationBar(this, false); // Hide the navigation bar
        }

        private async void OnLogInClicked(object sender, EventArgs e)
        {
            // Validate user input
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await DisplayAlert("Error", "Email and Password are required.", "OK");
                return;
            }

            try
            {
                // Call the Sign In API
                var response = await _httpClient.GetAsync($"/signin_action.php?email={Email}&password={Password}");
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<SignInResponse>(responseString);

                    if (result != null && result.Status == 200)
                    {
                        await DisplayAlert("Success", result.Message ?? "Logged in successfully.", "OK");

                        // Navigate to TaskPage after successful login
                        await Shell.Current.GoToAsync("//TaskPage");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Unexpected response from the server.", "OK");
                    }
                }
                else
                {
                    var error = JsonSerializer.Deserialize<ApiResponse>(responseString);
                    await DisplayAlert("Error", error?.Message ?? "Invalid credentials.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void SignUpTapped(object sender, EventArgs e)
        {
            // Navigate to the Sign Up page
            await Navigation.PushAsync(new SignUpPage());
        }
    }

    // Define the SignInResponse class to match the API response structure
    public class SignInResponse
    {
        public int Status { get; set; }
        public SignInData Data { get; set; }
        public string Message { get; set; }
    }

    // Define the SignInData class for the "data" field in the API response
    public class SignInData
    {
        public int Id { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string Email { get; set; }
        public string Timemodified { get; set; }
    }

    // Define the ApiResponse class for error responses
    //public class ApiResponse
    //{
    //    public int Status { get; set; }
    //    public string Message { get; set; }
    //}
}