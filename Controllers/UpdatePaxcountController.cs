using AHTAPI.Models;
using AHTAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AHTAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdatePaxcountController : ControllerBase
    {
        UpdatePaxcountRepository updatePaxcountRepository;
        public UpdatePaxcountController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            updatePaxcountRepository = new UpdatePaxcountRepository(connectionString);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PaxCounts data)
        {
            // Gọi phương thức AddDataToDB một cách bất đồng bộ
            Console.WriteLine(data.Id + " " + data.PaxCount);
            bool result = await updatePaxcountRepository.UpdateDataToDB(data);
            if (result)
            {
                return Ok("Data uploaded successfully.");
            }

            return BadRequest("Invalid data.");
        }
    }
}
