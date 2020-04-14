using System;
using System.Collections.Generic;
using System.Text;
using PictoManagementVocabulary;
using System.Data.SqlClient;
using System.Data;

namespace PictoManagementServer
{
    class DashboardRequestProcessor
    {
        private string _sqlCnStr = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Dashboards;Integrated Security=SSPI";
        private SqlConnection _sqlConnection;

        public DashboardRequestProcessor()
        {
            _sqlConnection = new SqlConnection(_sqlCnStr);
        }

        public List<string> GetDataFromDashboard(string dashboardName)
        {
            string query = "SELECT data FROM dashboards WHERE title LIKE @title";

            using (_sqlConnection)
            using (SqlCommand command = new SqlCommand(query, _sqlConnection))
            {
                command.Parameters.Add("title", SqlDbType.VarChar).Value = dashboardName;
                _sqlConnection.Open();
                int result = command.ExecuteNonQuery();
                if (result > 0)
                {
                    List<string> dashboards = new List<string>();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            dashboards.Add(dataReader.GetString(dataReader.GetOrdinal("data")));
                        }
                    }
                    return dashboards;
                }
            }
            return null;
        }
    }
}
