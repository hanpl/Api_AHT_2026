using AHTAPI.Models;
using Microsoft.Data.SqlClient;

namespace AHTAPI.Repositories
{
    public class FlightForBeltRepository
    {
        string connectionString;
        public FlightForBeltRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<FlightForBelt?> GetFlightBelt(string stand, int rollOn, int rollOff)
        {
            FlightForBelt? flight = null;

            string query = @"
                            SELECT TOP (1) F.[LineCode],F.[Number],F.[City],F.[Schedule],F.[Estimated],
                                   F.[Status],F.[Remark],F.[Belt],F.[DailyUpdateStand],
                                   COALESCE([Actual],[Estimated],[Schedule]) AS Time,A.Pvmd
                            FROM [MSMQFLIGHT].[dbo].[AHT_FlightInformation] AS F
                            LEFT JOIN [MSMQFLIGHT].[dbo].[AHT_AirlinesInformation] AS A
                                ON F.[LineCode] = A.[LineCode]
                            WHERE Adi='A'
                              AND DailyUpdateStand = @StandNumber
                              AND Status NOT IN ('','Cancelled')
                              AND DATEADD(MINUTE,@RollOn,COALESCE([Actual],[Estimated],[Schedule]))<GETDATE()
                              AND DATEADD(MINUTE,@RollOff,COALESCE([Actual],[Estimated],[Schedule]))>GETDATE()
                            ORDER BY Time DESC";

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@StandNumber", stand);
                cmd.Parameters.AddWithValue("@RollOn", rollOn);
                cmd.Parameters.AddWithValue("@RollOff", rollOff);

                await con.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        flight = new FlightForBelt
                        {
                            LineCode = reader["LineCode"]?.ToString(),
                            Number = reader["Number"]?.ToString(),
                            City = reader["City"]?.ToString(),
                            Schedule = reader["Schedule"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["Schedule"]),
                            Estimated = reader["Estimated"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["Estimated"]),
                            Time = reader["Time"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["Time"]),
                            Status = reader["Status"]?.ToString(),
                            Remark = reader["Remark"]?.ToString(),
                            Belt = reader["Belt"]?.ToString(),
                            Stand = reader["DailyUpdateStand"]?.ToString(),
                            Pvmd = reader["Pvmd"]?.ToString()
                        };
                    }
                }
            }
            return flight;
        }

    }
}
