using Microsoft.Data.SqlClient;
using System.Data;

namespace DashboardApi.Helpers
{
    public class DbHelper
    {
        private readonly string _conn;

        public DbHelper(string connectionString)
        {
            _conn = connectionString;
        }

        public List<Dictionary<string, object>> GetData(
            string sql,
            params SqlParameter[] parameters)
        {
            using SqlConnection conn = new SqlConnection(_conn);
            using SqlCommand cmd = new SqlCommand(sql, conn);

            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    if (p != null)
                        cmd.Parameters.Add(p);
                }
            }

            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            conn.Open();
            da.Fill(dt);

            return ConvertToList(dt);
        }


        private List<Dictionary<string, object>> ConvertToList(DataTable dt)
        {
            var list = new List<Dictionary<string, object>>();

            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();

                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] =
                        row[col] == DBNull.Value ? null : row[col];
                }

                list.Add(dict);
            }

            return list;
        }

        public (DateTime start, DateTime end) GetWindow(DateTime day)
        {
            var start = day.Date.AddHours(5);
            var end = start.AddDays(1).AddSeconds(-1);
            return (start, end);
        }
    }
}
