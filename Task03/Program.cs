using Avalonia;
using Avalonia.ReactiveUI;

namespace Task03
{
    
    public static class Program
    {
        public static void Main(string[] args)
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        private static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UseX11()
                .UseReactiveUI()
                .UseSkia()
                .LogToTrace();
        }
    }
}