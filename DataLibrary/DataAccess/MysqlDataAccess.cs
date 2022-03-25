using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace DataLibrary.DataAccess
{
    class MysqlDataAccess
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("dataAccess");
        public static string GetConnectionString(string connectionName = "DefaultConnection")
        {
            
            return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
        }

        public static List<T> LoadData<T>(string sql, object data)
        {
            using (IDbConnection cnn = new MySqlConnection(GetConnectionString()))
            {
                return cnn.Query<T>(sql, data).ToList();

            }
        }

        public static List<T> LoadData<T>(string sql)
        {
            using (IDbConnection cnn = new MySqlConnection(GetConnectionString()))
            {
                
                
                return cnn.Query<T>(sql).ToList();
            }

        }
        public static int SaveData<T>(string sql, T data)
        {
            using (MySqlConnection cnn = new MySqlConnection(GetConnectionString()))
            {
                return cnn.Execute(sql, data);
            }
        }
    }
}
