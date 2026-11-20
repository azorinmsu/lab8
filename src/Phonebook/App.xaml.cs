using System.Diagnostics;

namespace Phonebook
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Глобальный обработчик исключений
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Exception ex = (Exception)args.ExceptionObject;
                Debug.WriteLine($"Необработанное исключение: {ex.Message}");
                Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            };
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}