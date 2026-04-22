using AHTAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AHTAPI.Repositories
{
    public class StandsInformationRepository
    {
        string connectionString;
        public StandsInformationRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        #region Get Device By Ip
        public AHT_StandsInformation? GetDeviceByIp(string ip)
        {
            var data = GetDevice(ip);

            if (data.Rows.Count == 0)
                return null; 

            DataRow row = data.Rows[0];

            return new AHT_StandsInformation
            {
                Id = Convert.ToInt32(row["Id"]),
                Name = row["Name"].ToString(),
                Location = row["Location"].ToString(),
                Ip = row["Ip"].ToString(),
                Description = row["Description"].ToString(),
                Distance = row["Distance"].ToString(),
                RollOn = Convert.ToInt32(row["RollOn"]),
                RollOff = Convert.ToInt32(row["RollOff"]),
                ReloadPage = Convert.ToInt32(row["ReloadPage"]),
                En = row["En"].ToString(),
                Vn = row["Vn"].ToString(),
                Kr = row["Kr"].ToString(),
                Cn = row["Cn"].ToString(),
                ConnectionId = row["ConnectionId"].ToString(),
                Handler = row["Handler"].ToString(),
                EnTitle = row["EnTitle"].ToString(),
                VnTitle = row["VnTitle"].ToString(),
                KrTitle = row["KrTitle"].ToString(),
                CnTitle = row["CnTitle"].ToString()
            };
        }
        public DataTable GetDevice(string ip)
        {
            string query = "SELECT * FROM [MSMQFLIGHT].[dbo].[AHT_StandsInformation] WHERE Ip = '" + ip + "'";
            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            dataTable.Load(reader);
                        }
                    }
                    return dataTable;
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally { connection.Close(); }
            }
        }
        #endregion

        #region Get all devices
        public async Task<List<AHT_StandsInformation>> GetAll()
        {
            var devices = new List<AHT_StandsInformation>();


            string query = $@"SELECT *  FROM [MSMQFLIGHT].[dbo].[AHT_StandsInformation] order by Name";
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {

                await con.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        devices.Add(new AHT_StandsInformation
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Location = reader["Location"].ToString(),
                            Ip = reader["Ip"].ToString(),
                            Description = reader["Description"].ToString(),
                            Distance = reader["Distance"].ToString(),
                            RollOn = reader["RollOn"] is DBNull ? 0 : Convert.ToInt32(reader["RollOn"]),
                            RollOff = reader["RollOff"] is DBNull ? 0 : Convert.ToInt32(reader["RollOff"]),
                            ReloadPage = reader["ReloadPage"] is DBNull ? 0 : Convert.ToInt32(reader["ReloadPage"]),
                            En = reader["En"].ToString(),
                            Vn = reader["Vn"].ToString(),
                            Kr = reader["Kr"].ToString(),
                            Cn = reader["Cn"].ToString(),
                            ConnectionId = reader["ConnectionId"].ToString(),
                            Handler = reader["Handler"].ToString(),
                            EnTitle = reader["EnTitle"].ToString(),
                            VnTitle = reader["VnTitle"].ToString(),
                            KrTitle = reader["KrTitle"].ToString(),
                            CnTitle = reader["CnTitle"].ToString()
                        });
                    }
                }
            }
            return devices;
        }
        #endregion
    }
}
