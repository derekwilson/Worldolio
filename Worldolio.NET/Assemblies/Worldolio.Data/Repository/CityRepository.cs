using Dapper;
using System.Data;
using Worldolio.Data.Model;

namespace Worldolio.Data.Repository
{
    public interface ICityRepository
    {
        City? GetById(long id);

        ICollection<City> GetByIds(long[] ids);

        ICollection<City> GetAll();

        ICollection<City> GetNearbyCities(City c, Distance dist);
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
        private const string SQL_WHERE_ID_IN_SUFFIX = " WHERE cty.cty_id IN @IDS ";

        private const string SQL_ORDER_BY_SUFFIX = " ORDER BY cty.cty_displayname";

        private const string SQL_SELECT_ALL = SQL_SELECT + SQL_ORDER_BY_SUFFIX;
        private const string SQL_SELECT_BY_ID = SQL_SELECT + SQL_WHERE_ID_SUFFIX + SQL_ORDER_BY_SUFFIX;
        private const string SQL_SELECT_BY_IDS = SQL_SELECT + SQL_WHERE_ID_IN_SUFFIX + SQL_ORDER_BY_SUFFIX;

        private IConnectionFactory _connectionFactory;
        private ITimeZoneFactory _timeZoneFactory;

        public CityRepository(IConnectionFactory connectionFactory, ITimeZoneFactory timeZoneFactory)
        {
            _connectionFactory = connectionFactory;
            _timeZoneFactory = timeZoneFactory;
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

        public City? GetById(long id)
        {
            using (IDbConnection connection = _connectionFactory.GetOpenConnection())
            {
                var items = connection.Query<City, Country, DriveSide, City>(
                            SQL_SELECT_BY_ID,
                            MAP,
                            new { ID = id },
                            splitOn: "cty_id, cnt_iso2name, dsi_id"
                        );
                return items.FirstOrDefault();
            }
        }

        public ICollection<City> GetByIds(long[] ids)
        {
            using (IDbConnection connection = _connectionFactory.GetOpenConnection())
            {
                var items = connection.Query<City, Country, DriveSide, City>(
                            SQL_SELECT_BY_IDS,
                            MAP,
                            new { IDS = ids },
                            splitOn: "cty_id, cnt_iso2name, dsi_id"
                        );

                // this could be slow if the ids array is big
                var sorted = items.OrderBy(i => Array.IndexOf(ids, i.Id));
                return sorted.ToList();
            }
        }

        private City MAP(City cty, Country cnt, DriveSide dsi)
        {
            cnt.DriveSide = dsi;
            cty.Country = cnt;
            // we store the lat/long in integers in the DB
            cty.Position = new Position(cty.Latitude / 100.0, cty.Longitude / 100.0);
            cty.TimeZone = _timeZoneFactory.GetTimeZoneFromIanaName(cty.IanaTz);
            return cty;
        }

        /// <summary>
        /// Gets all the cities in a box drawn around the city
        /// </summary>
        /// <param name="dist">distance to the edges of the box</param>
        /// <returns>dataview of cities in the area</returns>
        public ICollection<City> GetNearbyCities(City c, Distance dist)
        {
            return GeoCalculator.GetCitiesInArea(this, c.Position, dist, dist);
        }
    }
}
