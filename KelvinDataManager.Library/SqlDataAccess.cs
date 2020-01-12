using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace KelvinDataManager.Library.Internal.DataAccess
{
    internal class SqlDataAccess // "Internal" meaning this class is not visible outside of the library
    {
        public string GetConnectionString(string name)
        {
            /**
             ConfigurationManager.ConnectionStrings[name].ConnectionString
            goes out to the Web.config or App.config gets the connection string with a matching
            name and returns that ConnectionString
             */
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        // Using Dapper, which is a micro ORM, allows us to talk to the DB get the info back and map 
        // that data into objects
        public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                List<T> rows = connection.Query<T>(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure).ToList();

                return rows;
            }
        }
        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Execute(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
