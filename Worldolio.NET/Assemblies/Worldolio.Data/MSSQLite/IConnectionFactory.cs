using System.Data;

namespace Worldolio.Data.MSSQLite
{
    public interface IConnectionFactory
    {
        IDbConnection GetOpenConnection();
    }
}
