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
    internal class SqlDataAccess : IDisposable // "Internal" meaning this class is not visible outside of the library
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

        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private Boolean isClosed = false;
        public void StartTransaction(string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();

            isClosed = false;
        }

        public void SaveDataInTransaction<T>(string storedProcedure, T parameters)
        {
            _connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure
                , transaction:_transaction);// pass in the ongoing transaction
        }

        public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
        {
            List<T> rows = _connection.Query<T>(storedProcedure, parameters,
                commandType: CommandType.StoredProcedure, transaction: _transaction).ToList();
            return rows;
        } 

        public void CommitTransaction()
        {
            _transaction?.Commit(); // if tranx succeed, we commit it; "?" is null check, if _trans is null, we 're not calling "Commit" method.
            
            _connection?.Close();
            isClosed = true;
        }

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _connection?.Close();
            isClosed = true;
        }

        public void Dispose()
        {
            if(!isClosed == true)
            {
                try {
                    CommitTransaction();
                } catch
                {
                    // Log this exception message
                }
                

            }
            _transaction = null;
            _connection = null;
        }
        // Open connection/start transaction method
        // load using the transx
        // save using the tranx
        // close connection/stop tranx method
        // Clean up connection
    }
}
