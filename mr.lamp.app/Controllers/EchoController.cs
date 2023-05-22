using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EchoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EchoController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            using var streamReader = new System.IO.StreamReader(Request.Body);
            var requestBody = await streamReader.ReadToEndAsync();

            if (string.IsNullOrEmpty(requestBody))
            {
                return BadRequest("Invalid JSON");
            }

            // Parse the JSON payload
            var jsonDocument = JsonDocument.Parse(requestBody);
            var responseJson = jsonDocument.RootElement.Clone();

            return Ok(responseJson);
        }
    }
}