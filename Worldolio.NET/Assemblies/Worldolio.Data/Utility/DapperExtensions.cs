using Dapper;
using System.Data;
using Worldolio.Data.Model;

namespace Worldolio.Data.Utility
{
    public static class DapperExtensions
    {
        public static void AttachMapper<T>()
        {
            var mapper = (SqlMapper.ITypeMap?)Activator.CreateInstance(typeof(ColumnAttributeTypeMapper<>).MakeGenericType(typeof(T)));
            SqlMapper.SetTypeMap(typeof(T), mapper);
        }

        public static void AttachMappers()
        {
            // if you miss out a type here then you will get the correct number of records/model objects but all will be empty
            AttachMapper<SchemaRevisionAudit>();
            AttachMapper<DriveSide>();
            AttachMapper<Country>();
            AttachMapper<City>();
        }

        public static void ExecuteNonQuery(this IDbConnection connection, string commandText)
        {
            // Ensure we have a connection
            if (connection == null)
            {
                throw new NullReferenceException("Please provide a connection");
            }

            // Ensure that the connection state is Open
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            // Use Dapper to execute the given query
            connection.Execute(commandText);
        }

    }
}
