using jbx.core.Models.Bot;
using jbx.core.Models.Rabbitmq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RestSharp;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace jbx.api.chat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BotController : ControllerBase
    {
        private readonly ILogger<BotController> _logger;
        private IWebHostEnvironment _env;
        private IMemoryCache _cache;

        public BotController(ILogger<BotController> logger, IMemoryCache memoryCache, IWebHostEnvironment env)
        {
            _logger = logger;
            _cache = memoryCache;
            _env = env;
        }

        [HttpGet]
        [Route("StooqSource")]
        [Authorize]
        public async Task<IActionResult> StooqSourceAsync([FromQuery] JobsityMessage message)
        {
            try
            {
                // Change         : Jobcity request to read data directly from api.
                // Change Date    : 2020/01/21 
                var stockName = message.Msg.Substring(7);
                var client = new RestClient($"https://stooq.com/q/l");
                var request = new RestRequest("?s=" + stockName + "&f=sd2t2ohlcv&h&e=csv", Method.Get);

                var response = await client.ExecuteGetAsync(request);
                if (response.IsSuccessful)
                {
                    if (!string.IsNullOrEmpty(response.Content))
                    {
                        var stooqs = new List<StooqDetail>();
                        using (var stream = new MemoryStream(response.RawBytes!))
                        using (var sr = new StreamReader(stream))
                        {
                            var isHeader = true;
                            while (!sr.EndOfStream)
                            {
                                var row = sr.ReadLine();
                                if (isHeader)
                                {
                                    isHeader = false;
                                    continue;
                                }

                                var l = row?.Split(',').ToArray();
                                if (l != null)
                                {
                                    if (l.Length > 0)
                                    {
                                        var item = new StooqDetail
                                        {
                                            Symbol = l[0],
                                            Date = l[1],
                                            Time = l[2],
                                            Open = double.Parse(l[3]),
                                            High = double.Parse(l[4]),
                                            Low = double.Parse(l[5]),
                                            Close = double.Parse(l[6]),
                                            Volume = int.Parse(l[7])
                                        };
                                        stooqs.Add(item);
                                    }
                                }
                            }
                        }
                        var stooq = stooqs.First();
                        var value = $"{stooq.Symbol.ToUpper()} quote is ${stooq.Open} per share.";
                        return Ok(new { IsSuccess = true, From = "bot", Data = value });
                    }
                    return Ok(new { IsSuccess = true, From = "bot", Data = $"Hello {message.User}, I can't help you with {stockName.ToUpper()}." });
                }
                return Ok(new { IsSuccess = true, From = "bot", Data = $"Hello {message.User}, I can't help you with {stockName.ToUpper()}." });
            }
            catch (Exception eX)
            {
                _logger.LogError(eX, eX.Message);
                return BadRequest(eX);
            }
        }
    }
}

