using System.Data;

namespace Worldolio.Data
{
    public interface IConnectionFactory
    {
        IDbConnection GetOpenConnection();
    }
}
