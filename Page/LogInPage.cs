using Microsoft.Maui.Controls;
using System.Net.Http;
using System.Text.Json;
using TodoListApp1.Models;

namespace TodoListApp1.Page
{
    public partial class LogInPage : ContentPage
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

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
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await DisplayAlert("Error", "Email and Password are required.", "OK");
                return;
            }

            try
            {
                var response = await _httpClient.GetAsync($"/signin_action.php?email={Email}&password={Password}");
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<SignInResponse>(responseString, JsonOptions);

                    if (result != null && result.Status == 200)
                    {
                        Preferences.Set("user_id", result.Data.Id);
                        await DisplayAlert("Success", result.Message ?? "Logged in successfully.", "OK");

                        // Navigate to TaskPage after successful login
                        Application.Current.MainPage = new AppShell();
                        await Shell.Current.GoToAsync("//TaskPage");
                    }
                    else
                    {
                        await DisplayAlert("Error", result?.Message ?? "Unexpected response from the server.", "OK");
                    }
                }
                else
                {
                    var error = JsonSerializer.Deserialize<ApiResponse>(responseString, JsonOptions);
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
            await Navigation.PushAsync(new SignUpPage());
        }
    }
    
    // Define the SignInData class for the "data" field in the API response


    // Define the ApiResponse class for error responses
 //   public class ApiResponse
 // {
   //     public int Status { get; set; }
     //   public string Message { get; set; }
    //}
}
