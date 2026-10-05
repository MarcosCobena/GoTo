using Avalonia;
using Avalonia.Browser;
using System.Runtime.Versioning;
using System.Threading.Tasks;

[assembly: SupportedOSPlatform("browser")]

namespace GoToStudio
{
    internal class Program
    {
        private static Task Main(string[] args) =>
            BuildAvaloniaApp().StartBrowserAppAsync("out");

        public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>();
    }
}
