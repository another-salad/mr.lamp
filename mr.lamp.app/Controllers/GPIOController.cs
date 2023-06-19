using System.Device.Gpio;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace GPIOAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GpioApiController : ControllerBase
    {
        private GpioController _gpio = new GpioController();
        private JsonDocument? jsonDoc;

        [HttpPost]
        public async Task<IActionResult> SetGpioValue()
        {
            using var streamReader = new System.IO.StreamReader(Request.Body);
            var requestBody = await streamReader.ReadToEndAsync();
            try
            {
                jsonDoc = JsonDocument.Parse(requestBody);
                JsonElement root = jsonDoc.RootElement;
                if (root.TryGetProperty("on", out JsonElement ledStatus))
                {
                    bool on = ledStatus.GetBoolean();
                    _gpio.OpenPin(18, PinMode.Output);
                    _gpio.Write(18, on ? PinValue.High : PinValue.Low);
                    _gpio.ClosePin(18);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception thrown {ex.Message}");
                return BadRequest();

            }
            finally
            {
                if (jsonDoc is not null)
                {
                    jsonDoc.Dispose();
                }
            }
        }
    }
}