using AHTAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AHTAPI.Repositories
{
    public class UpdateAmsFlightIdRepository
    {
        string connectionString;
        public UpdateAmsFlightIdRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<bool> UpdateDataToDB(AmsflightId data)
        {
            bool result = UpdateFlight(data);
            if (data == null) return false;
            return result;
        }

        public bool UpdateFlight(AmsflightId data)
        {
            //Console.WriteLine(rabbitMQ.FieldValue);
            string query = "UPDATE [MSMQFLIGHT].[dbo].[AHT_FlightInformation] SET Amslinkedflightid = @Amslinkedflightid WHERE Id = @Id";
            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Amslinkedflightid", data.Amslinkedflightid);
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
