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
        private LogSingleTon log = LogSingleTon.Instance;

        /// <summary>
        /// Constructor de la clase, genera la conexión con la base de datos.
        /// </summary>
        public DashboardRequestProcessor()
        {
            _sqlConnection = new SqlConnection(_sqlCnStr);
        }

        /// <summary>
        /// Prepara la inserción en la base de datos
        /// </summary>
        /// <param name="requestBody">Cuerpo de la petición en formato string</param>
        /// <returns>Retorna un array de string que es la request preparada para ser insertada en la base de datos</returns>
        public string[] PrepareRequestForInsert(string requestBody)
        {
            string[] requestForInsert = new string[2];
            string[] requestSplitted = requestBody.Split(",");


            string images = "";

            for (int i = 1; i < requestSplitted.Length; i++)
                images += requestSplitted[i] + ",";

            requestForInsert[0] = requestSplitted[0];
            requestForInsert[1] = images;

            return requestForInsert;
        }

        /// <summary>
        /// Obtiene los datos de la base de datos que es el dashboard solicitado
        /// </summary>
        /// <param name="dashboardName">Nombre del dashboard que se solicita</param>
        /// <returns>Devuelve una lista de strings que son todos los dashboard que coincidan con el nombre recibido</returns>
        public List<string> GetDataFromDashboard(string dashboardName)
        {
            string query = "SELECT data FROM dashboards WHERE title LIKE @title";
            List<string> dashboards = new List<string>();

            using (_sqlConnection)
            using (SqlCommand command = new SqlCommand(query, _sqlConnection))
            {
                command.Parameters.Add("title", SqlDbType.VarChar).Value = dashboardName;

                try
                {
                    _sqlConnection.Open();


                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                dashboards.Add(dataReader.GetString(dataReader.GetOrdinal("data")));
                            }
                            dataReader.Close();
                        }


                    }

                    _sqlConnection.Dispose();
                }
                catch (SqlException se)
                {
                    log.LogError("Error executing the query: " + se.StackTrace);
                    throw se.InnerException;
                }
            }
            if (dashboards.Count > 0)
                return dashboards;

            return null;
        }

        /// <summary>
        /// Inserta una nueva entrada en la base de datos con el nombre y las imágenes que componen el dashboard
        /// </summary>
        /// <param name="dashboardName">Nombre del dashboard</param>
        /// <param name="dashboardImages">Imágenes que componen el dashboard</param>
        public void InsertDataIntoDashboards (string dashboardName, string dashboardImages)
        {
            string query = "INSERT INTO dashboards (title, data) VALUES (@title, @|data)";

            using (_sqlConnection)
            using (SqlCommand command = new SqlCommand(query, _sqlConnection))
            {
                command.Parameters.Add("title", SqlDbType.VarChar).Value = dashboardName;
                command.Parameters.Add("data", SqlDbType.VarChar).Value = dashboardImages;

                try
                {
                    _sqlConnection.Open();
                
                    var result = command.ExecuteNonQuery();
                    _sqlConnection.Dispose();
                }
                catch (SqlException se)
                {
                    Console.WriteLine("Error during sql connection: {0}", se.Source);
                    throw;
                }
            }
        }
    }
}
