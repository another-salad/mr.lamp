using System.Device.Gpio;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading;

using static MorseCodeConverter;


namespace GPIOAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GpioApiController : ControllerBase
    {
        private GpioController _gpio = new GpioController();
        private JsonDocument? jsonDoc;

        private MorseCodeConverter converter = new MorseCodeConverter();

        [HttpPost]
        public async Task<IActionResult> SetGpioValue()
        {
            using var streamReader = new System.IO.StreamReader(Request.Body);
            var requestBody = await streamReader.ReadToEndAsync();
            try
            {
                jsonDoc = JsonDocument.Parse(requestBody);
                JsonElement root = jsonDoc.RootElement;
                if (root.TryGetProperty("message", out JsonElement message))
                {
                    string messageAsString = message.ToString();
                    Console.WriteLine(messageAsString);
                    string messageAsMorse = ConvertToMorseCode(messageAsString);
                    Console.WriteLine(messageAsMorse);
                    _gpio.OpenPin(18, PinMode.Output);
                    _gpio.Write(18, PinValue.Low); // Make sure we are off at the start
                    // hideous logic for now
                    foreach (char c in messageAsMorse)
                    {
                        Console.WriteLine(c);
                        if (c == '.')
                        {
                            _gpio.Write(18, PinValue.High);
                            Thread.Sleep(250);
                            _gpio.Write(18, PinValue.Low);
                            Thread.Sleep(1000);
                        }
                        else if (c == '-')
                        {
                            _gpio.Write(18, PinValue.High);
                            Thread.Sleep(1000);
                            _gpio.Write(18, PinValue.Low);
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            // must be a space
                            Thread.Sleep(1000);
                        }
                    }
                    // to confirm the end of the message, lets do a 3 quick flashes
                    for (int i = 0; i < 3; i++)
                    {
                        _gpio.Write(18, PinValue.High);
                        Thread.Sleep(250);
                        _gpio.Write(18, PinValue.Low);
                        Thread.Sleep(250);
                    }
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