using AHTAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AHTAPI.Repositories
{
    public class FidsInformationRepository
    {
        string connectionString;
        public FidsInformationRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        #region Get Device By Ip
        public AHT_FidsInformation? GetDeviceByIp(string ip)
        {
            var data = GetDevice(ip);

            if (data.Rows.Count == 0)
                return null; // Không tìm thấy IP

            DataRow row = data.Rows[0];

            return new AHT_FidsInformation
            {
                Id = Convert.ToInt32(row["Id"]),
                Name = row["Name"].ToString(),
                Location = row["Location"].ToString(),
                Ip = row["Ip"].ToString(),
                Description = row["Description"].ToString(),
                RollOn = row["RollOn"] is DBNull ? 0 : Convert.ToInt32(row["RollOn"]),
                RollOff = row["RollOn"] is DBNull ? 0 : Convert.ToInt32(row["RollOff"]),
                PageSize = row["RollOn"] is DBNull ? 0 : Convert.ToInt32(row["PageSize"]),
                MaxPages = row["RollOn"] is DBNull ? 0 : Convert.ToInt32(row["MaxPages"]),
                PageInterval = row["RollOn"] is DBNull ? 0 : Convert.ToInt32(row["PageInterval"]),
                ReloadInterval = row["RollOn"] is DBNull ? 0 : Convert.ToInt32(row["ReloadInterval"]),
                Mobilities = row["Mobilities"].ToString(),
                ConnectionId = row["ConnectionId"].ToString()
            };
        }
        public DataTable GetDevice(string ip)
        {
            string query = "SELECT * FROM [MSMQFLIGHT].[dbo].[AHT_FidsInformation] WHERE Ip = '" + ip + "'";
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
        public async Task<List<AHT_FidsInformation>> GetAll()
        {
            var devices = new List<AHT_FidsInformation>();


            string query = $@"SELECT *  FROM [MSMQFLIGHT].[dbo].[AHT_FidsInformation] order by Id";
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {

                await con.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        devices.Add(new AHT_FidsInformation
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Location = reader["Location"].ToString(),
                            Ip = reader["Ip"].ToString(),
                            Description = reader["Description"].ToString(),
                            RollOn = reader["RollOn"] is DBNull ? 0 : Convert.ToInt32(reader["RollOn"]),
                            RollOff = reader["RollOff"] is DBNull ? 0 : Convert.ToInt32(reader["RollOff"]),
                            PageSize = reader["PageSize"] is DBNull ? 0 : Convert.ToInt32(reader["PageSize"]),
                            MaxPages = reader["RollMaxPagesOn"] is DBNull ? 0 : Convert.ToInt32(reader["MaxPages"]),
                            PageInterval = reader["PageInterval"] is DBNull ? 0 : Convert.ToInt32(reader["PageInterval"]),
                            ReloadInterval = reader["ReloadInterval"] is DBNull ? 0 : Convert.ToInt32(reader["ReloadInterval"]),
                            Mobilities = reader["Mobilities"].ToString(),
                            ConnectionId = reader["ConnectionId"].ToString()
                        });
                    }
                }
            }
            return devices;
        }
        #endregion
    }
}
