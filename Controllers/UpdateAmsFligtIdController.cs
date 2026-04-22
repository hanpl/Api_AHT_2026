using AHTAPI.Models;
using AHTAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AHTAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateAmsFligtIdController : ControllerBase
    {
        UpdateAmsFlightIdRepository updateAmsFlightIdRepository;
        public UpdateAmsFligtIdController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            updateAmsFlightIdRepository = new UpdateAmsFlightIdRepository(connectionString);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AmsflightId data)
        {
            // Gọi phương thức AddDataToDB một cách bất đồng bộ
            Console.WriteLine(data.Id + " " + data.Amslinkedflightid);
            bool result = await updateAmsFlightIdRepository.UpdateDataToDB(data);
            if (result)
            {
                return Ok("Data uploaded successfully.");
            }

            return BadRequest("Invalid data.");
        }
    }
}
