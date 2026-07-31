using Dapper;
using System.Data;
using Worldolio.Data.Model;
using Worldolio.Data.MSSQLite;

namespace Worldolio.Data.Repository
{
    public interface ICountryRepository
    {
        Task<Country?> GetByIdAsync(string iso2);

        Task<ICollection<Country>> GetAllAsync();

        Task<Country?> GetByIdWithCitiesAsync(string iso2);

        Task<ICollection<Country>> GetAllWithCitiesAsync();

        Task<ICollection<Country>> FindByNameAsync(string nameSearch);
    }

    public class CountryRepository : ICountryRepository
    {
        private const string SQL_SELECT = @"
                    SELECT cnt.*, dsi.*
                    FROM cnt_country cnt
    				INNER JOIN dsi_driveside dsi ON dsi.dsi_id = cnt.cnt_dsi_id
                    ";
        private const string SQL_SELECT_WITH_CITIES = @"
                    SELECT cnt.*, dsi.*, cty.*
                    FROM cnt_country cnt
    				INNER JOIN dsi_driveside dsi ON dsi.dsi_id = cnt.cnt_dsi_id
                    INNER JOIN cty_city cty ON cnt.cnt_iso2name = cty.cty_cnt_iso2name                    
                    ";
        private const string SQL_WHERE_ID_SUFFIX = " WHERE cnt.cnt_iso2name = @ID ";
        private const string SQL_WHERE_NAME_SUFFIX = " WHERE cnt.cnt_displayname LIKE @SEARCH ";

        private const string SQL_ORDER_BY_SUFFIX = " ORDER BY cnt.cnt_displayname";
        private const string SQL_ORDER_BY_WITH_CITIES_SUFFIX = " ORDER BY cnt.cnt_displayname, cty.cty_displayname";

        private const string SQL_SELECT_ALL = SQL_SELECT + SQL_ORDER_BY_SUFFIX;
        private const string SQL_SELECT_BY_ID = SQL_SELECT + SQL_WHERE_ID_SUFFIX + SQL_ORDER_BY_SUFFIX;

        private const string SQL_SELECT_ALL_WITH_CITIES = SQL_SELECT_WITH_CITIES + SQL_ORDER_BY_WITH_CITIES_SUFFIX;
        private const string SQL_SELECT_BY_ID_WITH_CITIES = SQL_SELECT_WITH_CITIES + SQL_WHERE_ID_SUFFIX + SQL_ORDER_BY_WITH_CITIES_SUFFIX;

        private const string SQL_SELECT_BY_NAME = SQL_SELECT_WITH_CITIES + SQL_WHERE_NAME_SUFFIX + SQL_ORDER_BY_WITH_CITIES_SUFFIX;

        private IConnectionFactory _connectionFactory;

        public CountryRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ICollection<Country>> GetAllAsync()
        {
            using (IDbConnection connection = _connectionFactory.GetOpenConnection())
            {
                var items = await connection.QueryAsync<Country, DriveSide, Country>(
                            SQL_SELECT_ALL,
                            (cnt, dsi) =>
                            {
                                cnt.DriveSide = dsi;
                                return cnt;
                            },
                            splitOn: "cnt_iso2name, dsi_id"
                        );
                return items.ToList();
            }
        }

        public async Task<Country?> GetByIdAsync(string iso2)
        {
            using (IDbConnection connection = _connectionFactory.GetOpenConnection())
            {
                var items = await connection.QueryAsync<Country, DriveSide, Country>(
                            SQL_SELECT_BY_ID,
                            (cnt, dsi) =>
                            {
                                cnt.DriveSide = dsi;
                                return cnt;
                            },
                            new { ID = iso2 },
                            splitOn: "cnt_iso2name, dsi_id"
                        );
                return items.FirstOrDefault();
            }
        }

        public async Task<ICollection<Country>> GetAllWithCitiesAsync()
        {
            using (IDbConnection connection = _connectionFactory.GetOpenConnection())
            {
                var lookup = new Dictionary<string, Country>();
                var results = await connection.QueryAsync<Country, DriveSide, City, Country>(
                            SQL_SELECT_ALL_WITH_CITIES,
                            (cnt, dsi, cty) => {
                                cnt.DriveSide = dsi;
                                Country? country;
                                if (!lookup.TryGetValue(cnt.Iso2Name, out country))
                                    lookup.Add(cnt.Iso2Name, country = cnt);
                                cty.Country = country;
                                country.Cities.Add(cty);
                                return country;
                            },
                            splitOn: "cnt_iso2name, dsi_id, cty_id"
                        );
                ICollection<Country> resultList = lookup.Values;
                return resultList;
            }
        }

        public async Task<Country?> GetByIdWithCitiesAsync(string iso2)
        {
            using (IDbConnection connection = _connectionFactory.GetOpenConnection())
            {
                var lookup = new Dictionary<string, Country>();
                var results = await connection.QueryAsync<Country, DriveSide, City, Country>(
                            SQL_SELECT_BY_ID_WITH_CITIES,
                            (cnt, dsi, cty) => {
                                cnt.DriveSide = dsi;
                                Country? country;
                                if (!lookup.TryGetValue(cnt.Iso2Name, out country))
                                    lookup.Add(cnt.Iso2Name, country = cnt);
                                cty.Country = country;
                                country.Cities.Add(cty);
                                return country;
                            },
                            new { ID = iso2 },
                            splitOn: "cnt_iso2name, dsi_id, cty_id"
                        );
                ICollection<Country> resultList = lookup.Values;
                return resultList.FirstOrDefault();
            }
        }

        public async Task<ICollection<Country>> FindByNameAsync(string nameSearch)
        {
            using (IDbConnection connection = _connectionFactory.GetOpenConnection())
            {
                var wildcardSearch = $"%{nameSearch}%";
                var lookup = new Dictionary<string, Country>();
                var results = await connection.QueryAsync<Country, DriveSide, City, Country>(
                            SQL_SELECT_BY_NAME,
                            (cnt, dsi, cty) => {
                                cnt.DriveSide = dsi;
                                Country? country;
                                if (!lookup.TryGetValue(cnt.Iso2Name, out country))
                                    lookup.Add(cnt.Iso2Name, country = cnt);
                                cty.Country = country;
                                country.Cities.Add(cty);
                                return country;
                            },
                            new { SEARCH = wildcardSearch },
                            splitOn: "cnt_iso2name, dsi_id, cty_id"
                        );
                ICollection<Country> resultList = lookup.Values;
                return resultList;
            }
        }
    }
}
