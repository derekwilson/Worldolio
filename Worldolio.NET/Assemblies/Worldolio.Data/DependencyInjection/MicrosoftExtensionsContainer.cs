using Microsoft.Extensions.DependencyInjection;

namespace Worldolio.Data.DependencyInjection
{
    /// <summary>
    /// an injection container using the MicrosoftExtensions
    /// </summary>
    public class MicrosoftExtensionsContainer : IContainer
    {
        private ServiceCollection serviceCollection = new ServiceCollection();
        private ServiceProvider? serviceProvider = null;

        public void Register<TService, TImplementor>()
            where TService : class
            where TImplementor : class, TService
        {
            serviceCollection.AddTransient<TService, TImplementor>();
        }

        public void Register<TService, TImplementor>(Lifecycle lifecycle)
            where TService : class
            where TImplementor : class, TService
        {
            switch (lifecycle)
            {
                case Lifecycle.PerRequest:
                    serviceCollection.AddTransient<TService, TImplementor>();
                    break;

                case Lifecycle.PerThread:
                    throw new NotImplementedException();

                case Lifecycle.Singleton:
                    serviceCollection.AddSingleton<TService, TImplementor>();
                    break;

                default:
                    throw new NotImplementedException();
            }
        }


        public void Register(Type serviceTypeToRegisterAsSelf)
        {
            serviceCollection.AddTransient(serviceTypeToRegisterAsSelf);
        }

        public void Register<TService>(TService instance) where TService : class
        {
            serviceCollection.AddSingleton<TService>(instance);
        }

        public TService Resolve<TService>()
        {
            lock (this)
            {
                // it may be that we regret caching the provider - in which case we will need to be smarter
                if (serviceProvider == null)
                {
                    serviceProvider = serviceCollection.BuildServiceProvider();
                }
            }
            return serviceProvider.GetService<TService>() ?? throw new InvalidOperationException($"Cannot resolve service: {typeof(TService).FullName}");
        }
    }

}
