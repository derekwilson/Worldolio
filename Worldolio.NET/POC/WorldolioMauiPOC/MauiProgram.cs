using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Worldolio.Data.DependencyInjection;
using Worldolio.Data.Utility;
using WorldolioMauiPOC.Data;
using WorldolioMauiPOC.Logging;
using WorldolioMauiPOC.Utility;
using WorldolioMauiPOC.Views;

namespace WorldolioMauiPOC
{
    public static class MauiProgram
    {
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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // setup the DI container
            _container = Registration.GetEmptyContainer();
            _container.AttachExistingContainer(builder.Services);
            
            var dbPath = DatabaseHelper.GetDatabaseFilePath();
            _logger.Debug(() => $"DB = {dbPath}");

            Registration.RegisterFileDbConnection(_container, dbPath);
            Registration.RegisterServices(_container, _logger);

            // MAUI objects
            builder.Services.AddSingleton<IEnvironmentInformationProvider, EnvironmentInformationProvider>();

            // MAUI viewmodels
            builder.Services.AddSingleton<CityGridViewModel>();
            builder.Services.AddSingleton<CityGrid>();

            // database init
            DapperExtensions.AttachMappers();
            DatabaseHelper.CopyDatabaseToFileSystem(DatabaseHelper.GetDatabaseFilePath());

            return builder.Build();
        }

        #region Exception Handling

        private static void SetupExceptionHandler()
        {
            // Add handler for non-UI thread exceptions
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            // Add handler for background threads/tasks
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        }

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
