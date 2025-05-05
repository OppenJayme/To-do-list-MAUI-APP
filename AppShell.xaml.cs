using TodoListApp1.Page;

namespace TodoListApp1
{
    public partial class AppShell : Shell
    {
        public AppShell(string? initialRoute = null)
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
            Routing.RegisterRoute(nameof(TaskPage), typeof(TaskPage));
            Routing.RegisterRoute(nameof(AddTaskPage), typeof(AddTaskPage));

            if (!string.IsNullOrWhiteSpace(initialRoute))
            {
                Task.Run(async () =>
                {
                    await Task.Delay(100); // Small delay to ensure Shell is initialized
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await GoToAsync(initialRoute);
                    });
                });
            }
        }
    }
}