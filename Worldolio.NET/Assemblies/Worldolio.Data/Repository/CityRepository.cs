using Dapper;
using System.Data;
using Worldolio.Data.Model;

namespace Worldolio.Data.Repository
{
    public interface ICityRepository
    {
        ICollection<City> GetById(long id);

        ICollection<City> GetAll();
    }

    public class CityRepository : ICityRepository
    {
        private const string SQL_SELECT = @"
                    SELECT cty.*, cnt.*, dsi.*
                    FROM cty_city cty
    				INNER JOIN cnt_country cnt ON cnt.cnt_iso2name = cty.cty_cnt_iso2name
    				INNER JOIN dsi_driveside dsi ON dsi.dsi_id = cnt.cnt_dsi_id
                    ";
        private const string SQL_WHERE_ID_SUFFIX = " WHERE cty.cty_id = @ID ";

        private const string SQL_SELECT_ALL = SQL_SELECT;
        private const string SQL_SELECT_BY_ID = SQL_SELECT + SQL_WHERE_ID_SUFFIX;

        private IConnectionFactory _connectionFactory;

        public CityRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public ICollection<City> GetAll()
        {
            using (IDbConnection connection = _connectionFactory.GetOpenConnection())
            {
                var items = connection.Query<City, Country, DriveSide, City>(
                            SQL_SELECT_ALL,
                            MAP,
                            splitOn: "cty_id, cnt_iso2name, dsi_id"
                        );
                return items.ToList();
            }
        }

        public ICollection<City> GetById(long id)
        {
            using (IDbConnection connection = _connectionFactory.GetOpenConnection())
            {
                var items = connection.Query<City, Country, DriveSide, City>(
                            SQL_SELECT_BY_ID,
                            MAP,
                            new { ID = id },
                            splitOn: "cty_id, cnt_iso2name, dsi_id"
                        );
                return items.ToList();
            }
        }

        private City MAP(City cty, Country cnt, DriveSide dsi)
        {
            cnt.DriveSide = dsi;
            cty.Country = cnt;
            // we store the lat/long in integers in the DB
            cty.Position = new Position(cty.Latitude / 100.0, cty.Longitude / 100.0);
            cty.TimeZone = new Model.TimeZone(cty.IanaTz);
            return cty;
        }
    }
}
