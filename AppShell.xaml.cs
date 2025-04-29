using TodoListApp1.Page;

namespace TodoListApp1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage ));
            Routing.RegisterRoute(nameof(TaskPage), typeof(TaskPage));
        }
    }
}
    