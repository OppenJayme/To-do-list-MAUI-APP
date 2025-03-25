namespace TodoListApp1.Page
{
    public partial class SignUpPage
    {
        public SignUpPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        private void GoToSignIn(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new Page1());
            Navigation.PopAsync();
        }


        private void OnLabelTapped(object? sender, TappedEventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}