using System.Device.Gpio;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

using static MorseCodeConverter;


namespace GPIOAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GpioApiController : ControllerBase
    {
        private GpioController _gpio = new GpioController();
        private JsonDocument? jsonDoc;
        private int GpioPin = 18;
        private int LongWait = 1000;
        private int ShortWait = 250;

        private void CycleGpio (int waitTime)
        {
            _gpio.Write(GpioPin, PinValue.High);
            Thread.Sleep(waitTime);
            _gpio.Write(GpioPin, PinValue.Low);
        }

        private void BookEndMessage(Boolean FinalWait = true)
        {
            for (int i = 0; i < 3; i++)
            {
                CycleGpio(ShortWait);
                Thread.Sleep(ShortWait);
            }
            if (FinalWait)
            {
                Thread.Sleep(LongWait * 2);
            }
        }

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
                    string messageAsMorse = ConvertToMorseCode(messageAsString);
                    _gpio.OpenPin(GpioPin, PinMode.Output);
                    _gpio.Write(GpioPin, PinValue.Low); // Make sure we are off at the start
                    BookEndMessage();
                    foreach (char c in messageAsMorse)
                    {
                        switch (c)
                        {
                            case '.':
                                CycleGpio(ShortWait);
                                Thread.Sleep(LongWait);
                                break;
                            case '-':
                                CycleGpio(LongWait);
                                Thread.Sleep(LongWait);
                                break;
                            default:
                                // must be a space
                                Thread.Sleep(LongWait * 2);
                                break;
                        }
                    }
                    // to confirm the end of the message, lets do a 3 quick flashes
                    BookEndMessage(false);
                    _gpio.ClosePin(GpioPin);
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