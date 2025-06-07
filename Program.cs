using Microsoft.UI.Xaml;

namespace TrackpadSwipeApp
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var app = new App();

            app.Activated += (s, e) =>
            {
                var window = new InvisibleWindow();
                window.Activate();
            };

            app.Run();
        }
    }
}