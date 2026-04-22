using DashboardApi.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AHTAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirlinesController : ControllerBase
    {
        DbHelper _db;

        public AirlinesController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            _db = new DbHelper(connectionString);
        }


        [HttpGet("")]
        public IActionResult Arrivals()
        {
            string sql = @"SELECT 
                    a.Id, a.LineCode, a.Name, a.IsActive, a.CreatedAt,
                    l.Id, l.AirlineId, l.Location, l.ImageUrl, l.ImageName, l.UpdatedAt, l.UpdatedBy
                FROM AHT_AirlinesInformation a
                LEFT JOIN AirlineLogos l ON l.AirlineId = a.Id
                WHERE (null IS NULL OR a.LineCode LIKE null OR a.Name LIKE null)
                ORDER BY a.LineCode";

            var dt = _db.GetData(sql);

            return Ok(dt.ToList());
        }
    }
}
