using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Data
{
    public class DataContextDapper
    {
        private readonly string _connectionString = "";

        public DataContextDapper(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<T> LoadData<T>(string sql, DynamicParameters parameters)
        {
            IDbConnection dbConnection = new SqlConnection(_connectionString);
            if (parameters == null || !parameters.ParameterNames.Any())
                return dbConnection.Query<T>(sql);
            else
                return dbConnection.Query<T>(sql, parameters);
        }

        public T LoadDataSingle<T>(string sql, DynamicParameters parameters)
        {
            IDbConnection dbConnection = new SqlConnection(_connectionString);
            if(parameters == null || !parameters.ParameterNames.Any())
                return dbConnection.QuerySingle<T>(sql);
            else
                return dbConnection.QuerySingle<T>(sql, parameters);
        }

        public int ExecuteSqlWithRowCount(string sql, DynamicParameters parameters)
        {
            IDbConnection dbConnection = new SqlConnection(_connectionString);
            if (parameters == null || !parameters.ParameterNames.Any())
                return dbConnection.Execute(sql);
            else
                return dbConnection.Execute(sql, parameters);
        }

        public bool ExecuteSql(string sql, DynamicParameters parameters)
        {
            IDbConnection dbConnection = new SqlConnection(_connectionString);
            if (parameters == null || !parameters.ParameterNames.Any())
                return dbConnection.Execute(sql) > 0;
            else
                return dbConnection.Execute(sql, parameters) > 0;
        }
    }
}