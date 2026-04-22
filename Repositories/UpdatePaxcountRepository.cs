using AHTAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AHTAPI.Repositories
{
    public class UpdatePaxcountRepository
    {
        string connectionString;
        public UpdatePaxcountRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<bool> UpdateDataToDB(PaxCounts data)
        {
            bool result = UpdateFlight(data);
            if (data == null) return false;
            return result;
        }

        public bool UpdateFlight(PaxCounts data)
        {
            //Console.WriteLine(rabbitMQ.FieldValue);
            string query = "UPDATE [MSMQFLIGHT].[dbo].[AHT_FlightInformation] SET PaxCount = @PaxCount WHERE Id = @Id";
            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PaxCount", data.PaxCount);
                        command.Parameters.AddWithValue("@Id", data.Id);
                        command.ExecuteNonQuery();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error2: " + ex.Message);
                    return false;
                }
                finally { connection.Close(); }
            }

        }
    }
}
