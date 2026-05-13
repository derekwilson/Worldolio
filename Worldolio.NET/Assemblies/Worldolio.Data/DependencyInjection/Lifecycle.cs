namespace Worldolio.Data.DependencyInjection
{
    ///<summary>
    /// The lifecycle to use when resolving objects.
    ///</summary>
    public enum Lifecycle
    {
        ///<summary>
        /// Create a new object on each request
        ///</summary>
        PerRequest,

        ///<summary>
        /// Create one instance of the class per thread
        ///</summary>
        PerThread,

        ///<summary>
        /// Create a single instance for the whole application
        ///</summary>
        Singleton,
    }

}
