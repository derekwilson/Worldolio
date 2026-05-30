using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Worldolio.Data.DependencyInjection;
using Worldolio.Data.Utility;
using WorldolioMauiPOC.Data;
using WorldolioMauiPOC.Logging;
using WorldolioMauiPOC.Utility;
using WorldolioMauiPOC.ViewModels.About;
using WorldolioMauiPOC.ViewModels.CityGrid;
using WorldolioMauiPOC.ViewModels.Plan;
using WorldolioMauiPOC.Views;

namespace WorldolioMauiPOC
{
    public static class MauiProgram
    {
        public static IServiceProvider Services { get; private set; } = null!;

        private static Worldolio.Data.Logging.ILogger _logger = null!;
        private static Worldolio.Data.DependencyInjection.IContainer _container = null!;

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // configure the logging and test that its woking
            var env = new EnvironmentInformationProvider();
            var loggerFactory = new NLogMauiLoggerFactory();        // this will also configure NLog
            _logger = loggerFactory.Logger;
            _logger.Info(() => $"WorldolioMauiPOC, v{env.GetAppVersion()}, {env.GetPackageName()}, Running on .NET CLR: {Environment.Version.ToString()}");
            SetupExceptionHandler();

            // attach logging to maui
            builder.Logging.ClearProviders();
            builder.Logging.AddNLog();

            builder
                .UseMauiApp<App>()
                // Initialize the .NET MAUI Community Toolkit by adding the below line of code
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIconsRegular");
                    fonts.AddFont("MaterialSymbolsOutlined.ttf", "MaterialSymbolsOutlined");
                    fonts.AddFont("MaterialSymbolsRounded.ttf", "MaterialSymbolsRounded");
                    fonts.AddFont("MaterialSymbolsSharp.ttf", "MaterialSymbolsSharp");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // setup the DI container
            _container = Registration.GetEmptyContainer();
            _container.AttachExistingContainer(builder.Services);
            
            // add our services from Worldolio.Data into the container
            var dbPath = DatabaseHelper.GetDatabaseFilePath();
            _logger.Debug(() => $"DB = {dbPath}");
            Registration.RegisterFileDbConnection(_container, dbPath);
            Registration.RegisterServices(_container, _logger);

            // MAUI objects
            builder.Services.AddSingleton<IEnvironmentInformationProvider, EnvironmentInformationProvider>();
            builder.Services.AddSingleton<INavigationHelper, NavigationHelper>();

            // MAUI viewmodels
            builder.Services.AddSingleton<CityGridViewModel>();
            builder.Services.AddSingleton<CityGrid>();
            builder.Services.AddSingleton<PlanViewModel>();
            builder.Services.AddSingleton<Plan>();
//            builder.Services.AddSingleton<MoonViewModel>();
//            builder.Services.AddSingleton<Moon>();

            builder.Services.AddTransient<AboutViewModel>();
            builder.Services.AddTransient<About>();

            // database init
            DapperExtensions.AttachMappers();
            DatabaseHelper.CopyDatabaseToFileSystem(_logger, DatabaseHelper.GetDatabaseFilePath());

            // register routes
            Routing.RegisterRoute(nameof(About), typeof(About));

            var app = builder.Build();
            MauiProgram.Services = app.Services;
            return app;

        }

        #region Exception Handling

        private static void SetupExceptionHandler()
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
            _logger?.LogException(() => "AndroidEnvironment_UnhandledExceptionRaiser", e.Exception);
        }
#endif

#if WINDOWS
        private static void Ui_Current_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            _logger?.LogException(() => "Ui_Current_UnhandledException", e.Exception);
        }
#endif

        private static void TaskSchedulerOnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            _logger?.LogException(() => "TaskSchedulerOnUnobservedTaskException", e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception ?? new Exception("EXCEPTION NOT PROVIDED");
            _logger?.LogException(() => "CurrentDomain_UnhandledException", ex);
        }

#endregion
    }
}
