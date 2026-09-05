using Microsoft.Extensions.Logging;

namespace MauiPOC
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialSymbolsOutlined.ttf", "MaterialSymbolsOutlined");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            SetupExceptionHandler(app);

            return app;
        }

        #region Exception Handling

        private static void SetupExceptionHandler(MauiApp app)
        {
            // Add handler for non-UI thread exceptions
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            // Add handler for background threads/tasks
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

#if WINDOWS
            Microsoft.UI.Xaml.Application.Current.UnhandledException += Ui_Current_UnhandledException;
#elif ANDROID
            // For Android:
            // All exceptions will flow through Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser,
            // and NOT through AppDomain.CurrentDomain.UnhandledException

            Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser;
#endif
        }

#if ANDROID
        private static void AndroidEnvironment_UnhandledExceptionRaiser(object? sender, Android.Runtime.RaiseThrowableEventArgs e)
        {
            Console.WriteLine(e.ToString());
            //_logger?.LogException(() => "AndroidEnvironment_UnhandledExceptionRaiser", e.Exception);
        }
#endif

#if WINDOWS
        private static void Ui_Current_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ToString());
            //_logger?.LogException(() => "Ui_Current_UnhandledException", e.Exception);
        }
#endif

        private static void TaskSchedulerOnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            Console.WriteLine(e.ToString());
            //_logger?.LogException(() => "TaskSchedulerOnUnobservedTaskException", e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception ?? new Exception("EXCEPTION NOT PROVIDED");
            Console.WriteLine(ex.ToString());
            //_logger?.LogException(() => "CurrentDomain_UnhandledException", ex);
        }

        #endregion
    }
}
