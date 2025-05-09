using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Platform;
using TodoListApp1.Page; 

namespace TodoListApp1
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

#if ANDROID 
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (h, v) =>
            {
                h.PlatformView.BackgroundTintList =
                Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToPlatform());
            });
#endif
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            
            return new Window(new NavigationPage(new LogInPage()));
        }
    }
}
