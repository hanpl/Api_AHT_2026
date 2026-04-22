using AHTAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AHTAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class FlighsLinkController : ControllerBase
    {
        FlightLinksRepository flightLinksRepository;
        public FlighsLinkController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            flightLinksRepository = new FlightLinksRepository(connectionString);
        }
        [HttpGet("by-day")]
        public async Task<IActionResult> GetByDay([FromQuery] DateTime dateStart, DateTime dateEnd)
        {
            var flights = await flightLinksRepository.GetFlightsByDay(dateStart, dateEnd);
            return Ok(flights);
        }
    }
}
