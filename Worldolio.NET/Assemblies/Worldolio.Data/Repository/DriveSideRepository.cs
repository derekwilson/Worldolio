using System.Data;
using Dapper;
using Worldolio.Data.Model;

namespace Worldolio.Data.Repository
{
    public interface IDriveSideRepository
    {
        ICollection<DriveSide> GetById(long id);

        ICollection<DriveSide> GetAll();
    }

    public class DriveSideRepository : IDriveSideRepository
    {
        private const string SQL_SELECT = @"
                    SELECT dsi.*
                    FROM dsi_driveside dsi
                    ";
        private const string SQL_WHERE_ID_SUFFIX = " WHERE dsi.dsi_id = @ID ";

        private const string SQL_SELECT_ALL = SQL_SELECT;
        private const string SQL_SELECT_BY_ID = SQL_SELECT + SQL_WHERE_ID_SUFFIX;

        private IConnectionFactory _connectionFactory;


        public DriveSideRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public ICollection<DriveSide> GetAll()
        {
            using (IDbConnection connection = _connectionFactory.GetOpenConnection())
            {
                return connection.Query<DriveSide>(SQL_SELECT_ALL).ToList();
            }
        }

        public ICollection<DriveSide> GetById(long id)
        {
            using (IDbConnection connection = _connectionFactory.GetOpenConnection())
            {
                return connection.Query<DriveSide>(SQL_SELECT_BY_ID,new { ID = id }).ToList();
            }
        }


    }
}
