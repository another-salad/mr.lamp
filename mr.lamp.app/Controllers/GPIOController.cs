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

        [HttpPost]
        public async Task<IActionResult> SetGpioValue()
        {
            _gpio.OpenPin(18, PinMode.Output);
            using var streamReader = new System.IO.StreamReader(Request.Body);
            var requestBody = await streamReader.ReadToEndAsync();
            var jsonDocument = JsonDocument.Parse(requestBody);
            foreach (var property in jsonDocument.RootElement.EnumerateObject()) {
                string key = property.Name;
                JsonElement jsonValue = property.Value;
                var value = jsonValue.ToString();
                bool realVal = bool.Parse(value);
                _gpio.Write(18, realVal ? PinValue.High : PinValue.Low);
            }
            return Ok();
        }
    }
}