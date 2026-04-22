using AHTAPI.Models;
using AHTAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AHTAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StandInformationController : ControllerBase
    {
        StandsInformationRepository standsInformationRepository;
        public StandInformationController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            standsInformationRepository = new StandsInformationRepository(connectionString);
        }


        [HttpGet]
        public ActionResult<AHT_StandsInformation> GetFlightbyC()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            Console.WriteLine("ipAddress:  " + ipAddress);
            var device = standsInformationRepository.GetDeviceByIp(ipAddress);
            if (device == null)
                return NotFound("Không tìm thấy thiết bị theo IP");
            return Ok(device);
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAll()
        {
            var flights = await standsInformationRepository.GetAll();
            return Ok(flights);
        }
    }
}
