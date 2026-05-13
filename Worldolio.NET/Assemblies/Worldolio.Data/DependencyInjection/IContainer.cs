namespace Worldolio.Data.DependencyInjection
{

    /// <summary>
    /// supports the ability to register objects in an IoC container
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// We need to use an existing container, 
        /// for example MVC or MAUI have an existing container and we need to add services to that conatiner
        /// we must call this before registering any services, otherwise they will be added to the wrong container
        /// </summary>
        /// <typeparam name="TContainer">Container to use, must be compatable with the DI implementation</typeparam>
        /// <param name="container"></param>
        void AttachExistingContainer(object container);

        /// <summary>
        /// register a service
        /// </summary>
        /// <typeparam name="TService">the service to be registered, usually an interface</typeparam>
        /// <typeparam name="TImplementor">the concrete implementation</typeparam>
        void Register<TService, TImplementor>()
            where TService : class
            where TImplementor : class, TService;

        /// <summary>
        /// register a service
        /// </summary>
        /// <typeparam name="TService">the service to be registered, usually an interface</typeparam>
        /// <typeparam name="TImplementor">the concrete implementation</typeparam>
        /// <param name="lifecycle">The lifecycle of the registered implementation</param>
        void Register<TService, TImplementor>(Lifecycle lifecycle)
            where TService : class
            where TImplementor : class, TService;

        ///<summary>
        /// Register a type as both the service type and implementing type.
        ///</summary>
        ///<param name="serviceTypeToRegisterAsSelf">The service/implementing type to register</param>
        void Register(Type serviceTypeToRegisterAsSelf);

        ///<summary>
        /// Register an instance as a service.
        ///</summary>
        ///<param name="instance">The service/implementing instance to register</param>
        void Register<TService>(TService instance) where TService : class;

        ///<summary>
        /// Resolve a service
        ///</summary>
        ///<typeparam name="TService"></typeparam>
        ///<returns></returns>
        TService Resolve<TService>();
    }

}
