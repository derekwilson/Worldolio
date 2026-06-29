using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worldolio.Data.Repository;

namespace WorldolioCLI
{
    internal static class DatabaseHelper
    {
        const string DB_PATH_NAME = "./worldolio.sqlite";

        static public string GetDatabaseFilePath()
        {
            return DB_PATH_NAME;
        }

        static public async Task<string> GetDatabaseVersion(ISchemaRevisionAuditRepository sraRepository)
        {
            var versions = sraRepository.GetDatabaseSchemaVersions();
            var sra = await sraRepository.GetAllAsync();
            var dbDate = sra.FirstOrDefault()?.Timestamp.ToString();
            return $"Schema: {versions.Item2}, {dbDate}";
        }
    }
}
