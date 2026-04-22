using AHTAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace AHTAPI.Repositories
{
    public class FlightLinksRepository
    {
        string connectionString;
        public FlightLinksRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public string? FormatCounters(string? countersRaw)
        {
            if (string.IsNullOrWhiteSpace(countersRaw))
                return null;

            var parts = countersRaw.Split(',')
                                    .Select(x => x.Trim())
                                    .Where(x => !string.IsNullOrEmpty(x))
                                    .Select(counter => counter.StartsWith("M", StringComparison.OrdinalIgnoreCase)
                                        ? counter // Mobility, giữ nguyên
                                        : "C" + counter); // Thêm chữ C

            return string.Join(",", parts);
        }

        public DateTime? ConvertToFormattedDate(DateTime? input)
        {
            if (input == null) return null;

            string formatted = input.Value.ToString("M/d/yyyy HH:mm");
            return DateTime.ParseExact(formatted, "M/d/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
        }
        //public string? ConvertToFormattedDate(DateTime? input)
        //{
        //    if (input == null) return null;
        //    return input.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
        //}
        public async Task<List<FlightLinks>> GetFlightsByDay(DateTime dateStart, DateTime dateEnd)
        {
            var flights = new List<FlightLinks>();

            var query = @"WITH FlightsInRange AS (SELECT * FROM [AHT_FlightInformation]
                                WHERE Schedule >= DATEADD(HOUR, 5, CAST(@TargetDateStart AS DATETIME))
                                AND Schedule <  DATEADD(HOUR, 5, DATEADD(DAY, 1, CAST(@TargetDateEnd AS DATETIME)))),
                            ArrivalWithShare AS (
                                SELECT A.*, CShares.CodeshareList AS ArrivalCodeShares
                                FROM FlightsInRange A LEFT JOIN (
                                    SELECT IdFlightInformation, STRING_AGG(CONCAT(LineCode, Number), ', ') AS CodeshareList
                                    FROM [MSMQFLIGHT].[dbo].[AHT_CodeShare]
                                    GROUP BY IdFlightInformation ) CShares ON A.Id = CShares.IdFlightInformation
                                WHERE A.Adi = 'A' and A.Status <> ''),
                            DepartureWithShare AS (
                                SELECT D.*,  CShares.CodeshareList AS DepartureCodeShares
                                FROM FlightsInRange D LEFT JOIN (SELECT IdFlightInformation, STRING_AGG(CONCAT(LineCode, Number), ', ') AS CodeshareList
                                    FROM [MSMQFLIGHT].[dbo].[AHT_CodeShare]
                                    GROUP BY IdFlightInformation) CShares ON D.Id = CShares.IdFlightInformation
                                WHERE D.Adi = 'D' and D.Status <> '')
                            SELECT
                                A.Totalpax as Aircraft, A.Aircraft AS 'A/C Type', A.Transit24 AS 'Arr!!!',CONCAT(A.LineCode, A.Number) AS 'Arr Flight',A.PaxCount AS Config,A.Transit24 AS 'ARR Callsign',
	                            A.Schedule As STA, A.Domesticintcode As ArrType, A.City AS 'From', A.Qualifier AS Qual_ARR, A.Status as Status_ARR, A.Remark as Remark_ARR, 
                                A.Mcat AS MCAT, A.Estimated As ETA, A.Actual AS ATA, A.ChockOn,
	                            A.Totalpax As EstDeliveryTime, A.Totalpax as ArrPax, CASE when A.Belt IS NOT NULL AND A.Belt <> '' THEN CONCAT('B',A.Belt) ELSE NULL END As Carousel, A.Comment As ARR_Comments, A.DailyUpdateStand AS ArrStand, 
                                A.ArrivalCodeShares AS ArrCodeShares,
                                D.Acu45From as 'Dep!!!', D.Aircraft AS 'A/C DepType', CONCAT(D.LineCode, D.Number) as DepFlight, D.PaxCount as DepConfig, D.Acu45From as DepCallsign , D.Schedule as STD, 
                                D.Domesticintcode as DepType, D.City as 'To', D.Qualifier as Qual_DEP, 
	                            D.Status as Status_DEP, D.Remark as Remark_DEP , D.Mcdt as MCDT, D.Estimated as ETD, D.ChockOff ,D.Actual as ATD, D.Totalpax as DepPax, 
                                D.Gate, D.CheckInCounters as Counters, D.Comment as DEP_Comments, 
	                            D.DailyUpdateStand as DepStand, D.DepartureCodeShares AS DepCodeShares, 
	                            CASE  WHEN D.RowFrom IS NOT NULL AND D.RowFrom <> ''  AND TRY_CAST(D.RowFrom AS INT) <= 27 THEN 'DC1'
                                   WHEN D.RowFrom IS NOT NULL AND D.RowFrom <> '' AND TRY_CAST(D.RowFrom AS INT) > 27 THEN 'DC2' ELSE NULL END AS Chutes
                            FROM ArrivalWithShare A FULL OUTER JOIN DepartureWithShare D ON A.TurnId = D.TurnId AND A.Adi = 'A' AND D.Adi = 'D'
                            WHERE (A.Adi = 'A' OR D.Adi = 'D')  order by CASE WHEN D.Schedule IS NULL THEN 1 ELSE 0 END, D.Schedule ASC";

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.Add(new SqlParameter("@TargetDateStart", SqlDbType.Date) { Value = dateStart });
                command.Parameters.Add(new SqlParameter("@TargetDateEnd", SqlDbType.Date) { Value = dateEnd });

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var flight = new FlightLinks
                        {
                            Aircraft = reader["Aircraft"]?.ToString(),
                            ACType = reader["A/C Type"]?.ToString() != "" ? reader["A/C Type"]?.ToString() : reader["A/C DepType"]?.ToString(),
                            Transit24_ARR = reader["Arr!!!"]?.ToString(),
                            ArrFlight = reader["Arr Flight"]?.ToString(),
                            ArrConfig = reader["Config"]?.ToString() != "" ? reader["Config"].ToString() : reader["DepConfig"]?.ToString(),
                            ArrCallsign = reader["ARR Callsign"]?.ToString(),
                            STA = ConvertToFormattedDate(reader["STA"] as DateTime?),
                            ArrType = reader["ArrType"]?.ToString(),
                            From = reader["From"]?.ToString(),
                            Qual_ARR = reader["Qual_ARR"]?.ToString(),
                            Status_ARR = reader["Status_ARR"]?.ToString(),
                            Remark_ARR = reader["Remark_ARR"]?.ToString(),
                            MCAT = ConvertToFormattedDate((reader["ATA"] as DateTime?) ?? (reader["ETA"] as DateTime?) ?? (reader["STA"] as DateTime?)),
                            ETA = ConvertToFormattedDate(reader["ETA"] as DateTime?),
                            ATA = ConvertToFormattedDate(reader["ATA"] as DateTime?),
                            ChocksOn = ConvertToFormattedDate(reader["ChockOn"] as DateTime?),
                            EstDeliveryTime = reader["EstDeliveryTime"] as int?,
                            ArrPax = reader["ArrPax"]?.ToString(),
                            Carousel = reader["Carousel"]?.ToString(),
                            ARR_Comments = reader["ARR_Comments"]?.ToString(),
                            ArrStand = reader["ArrStand"]?.ToString(),
                            ArrCodeShares = reader["ArrCodeShares"]?.ToString(),

                            Transit24_DEP = reader["Dep!!!"]?.ToString(),
                            DepFlight = reader["DepFlight"]?.ToString(),
                            DepConfig = reader["DepConfig"]?.ToString() != "" ? reader["DepConfig"]?.ToString() : reader["Config"]?.ToString(),
                            DepCallsign = reader["DepCallsign"]?.ToString(),
                            STD = ConvertToFormattedDate(reader["STD"] as DateTime?),
                            DepType = reader["DepType"]?.ToString(),
                            To = reader["To"]?.ToString(),
                            Qual_DEP = reader["Qual_DEP"]?.ToString(),
                            Status_DEP = reader["Status_DEP"]?.ToString(),
                            Remark_DEP = reader["Remark_DEP"]?.ToString(),
                            MCDT = ConvertToFormattedDate((reader["ATD"] as DateTime?)?? (reader["ETD"] as DateTime?)?? (reader["STD"] as DateTime?)),
                            ETD = ConvertToFormattedDate(reader["ETD"] as DateTime?),
                            ChocksOff = ConvertToFormattedDate(reader["ChockOff"] as DateTime?),
                            ATD = ConvertToFormattedDate(reader["ATD"] as DateTime?),
                            DepPax = reader["DepPax"]?.ToString(),
                            Gate = "G"+reader["Gate"]?.ToString(),
                            Counters = FormatCounters(reader["Counters"]?.ToString()),
                            DEP_Comments = reader["DEP_Comments"]?.ToString(),
                            DepStand = reader["DepStand"]?.ToString(),
                            DepCodeShares = reader["DepCodeShares"]?.ToString(),
                            Chutes = reader["Chutes"]?.ToString(),
                        };

                        flights.Add(flight);
                    }
                }
            }

            return flights;
        }
    }
}
