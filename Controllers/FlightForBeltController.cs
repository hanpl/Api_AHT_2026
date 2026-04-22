using AHTAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AHTAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightForBeltController : ControllerBase
    {
        FlightForBeltRepository flightForBeltRepository;
        public FlightForBeltController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            flightForBeltRepository = new FlightForBeltRepository(connectionString);
        }

        [HttpGet]
        public async Task<IActionResult> GetFlightFidsDep(string stand, int rollOn, int rollOff)
        {
            var flights = await flightForBeltRepository.GetFlightBelt(stand, rollOn, rollOff);
            return Ok(flights);
        }
    }
}
