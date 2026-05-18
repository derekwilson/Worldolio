using Worldolio.Data.Logging;
using Worldolio.Data.Model;
using Worldolio.Data.MSSQLite;
using Worldolio.Data.Repository;
using Worldolio.Data.Utility;

namespace Worldolio.Data.DependencyInjection
{
    public static class Registration
    {
        public static IContainer GetEmptyContainer()
        {
            return new MicrosoftExtensionsContainer();
        }

        public static void RegisterFileDbConnection(IContainer container, string filename)
        {
            var connectionFactory = new LocalFileDbConnectionFactory(filename);
            container.Register<IConnectionFactory>(connectionFactory);
        }

        public static void RegisterServices(IContainer container, ILogger? logger)
        {
            if (logger != null)
            {
                container.Register<ILogger>(logger);
            }
            container.Register<ISystemTimeProvider, SystemTimeProvider>();
            container.Register<ITimeZoneFactory, TimeZoneFactory>();

            // data
            container.Register<ISchemaRevisionAuditRepository, SchemaRevisionAuditRepository>();
            container.Register<IDriveSideRepository, DriveSideRepository>();
            container.Register<ICountryRepository, CountryRepository>();
            container.Register<ICityRepository, CityRepository>();
        }
    }
}