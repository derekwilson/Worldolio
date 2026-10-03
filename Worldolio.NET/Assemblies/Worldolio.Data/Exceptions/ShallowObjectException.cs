namespace Worldolio.Data.Exceptions
{
    /// <summary>
    /// the domain object was not completely loaded and the operation is not possible
    /// </summary>
    [Serializable]
    public class ShallowObjectException : System.Exception
    {
        public ShallowObjectException()
            : base()
        { }

        public ShallowObjectException(string message)
            : base(message)
        { }

        public ShallowObjectException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
