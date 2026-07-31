using Worldolio.Data.DependencyInjection;
using Worldolio.Data.Logging;
using Worldolio.Data.Repository;
using Worldolio.Data.Utility;

namespace WorldolioCLI
{
    internal class Program
    {
        private static IContainer _container = null!;
        private static ILogger _logger = null!;
        private static ICityRepository _citiesRepository = null!;
        private static ICountryRepository _countriesRepository = null!;
        private static ISchemaRevisionAuditRepository _sraRepository = null!;

        private static async Task Main(string[] args)
        {
            ApplicationHelper.DisplayBanner();
            ApplicationHelper.DisplayEnvironment();

            Init();

            var dbVersion = await DatabaseHelper.GetDatabaseVersion(_sraRepository);
            ApplicationHelper.OutputToConsole($"Database: {dbVersion}");

            if (args.Length < 2)
            {
                ApplicationHelper.DisplayUsage();
                Environment.Exit(1);
            }

            ApplicationHelper.Command command = ApplicationHelper.GetCommand(args[0]);
            switch (command)
            {
                case ApplicationHelper.Command.Unknown:
                {
                    ApplicationHelper.DisplayUsage();
                    Environment.Exit(1);
                    break;
                }
                case ApplicationHelper.Command.CityList:
                {
                    //long[] cityIds = [429, 458, 252, 477, 324, 79, 320, 279, 382, 180, 351];
                    long[] cityIds = ApplicationHelper.GetLongList(args[1]);
                    await CityHelper.DisplayCityGrid(_citiesRepository, cityIds, false);
                    break;
                }
                case ApplicationHelper.Command.Find:
                {
                    await CityHelper.FindCities(_countriesRepository, _citiesRepository, args[1]);
                    break;
                }
            }
        }

        static void Init()
        {
            var loggerFactory = new NLogLoggerFactory();
            _logger = loggerFactory.Logger;
            _logger.Info(() => $"WorldolioCLI, v{ApplicationHelper.GetCodeVersion()}");
            SetupExceptionHandler();

            DapperExtensions.AttachMappers();
            _container = Registration.GetEmptyContainer();
            Registration.RegisterFileDbConnection(_container, DatabaseHelper.GetDatabaseFilePath());
            Registration.RegisterServices(_container, _logger);

            var diLogger = _container.Resolve<ILogger>();
            //diLogger.Info(() => $"DI init OK");

            _citiesRepository = _container.Resolve<ICityRepository>();
            _countriesRepository = _container.Resolve<ICountryRepository>();
            _sraRepository = _container.Resolve<ISchemaRevisionAuditRepository>();
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
