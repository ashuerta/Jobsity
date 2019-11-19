using Jobsity.Core.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileIO = System.IO;

namespace Jobsity.Service.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
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
        [Route("ResponseMsg")]
        [Authorize]
        public async Task<IActionResult> ResponseMsgAsync([FromQuery] JobsityMessage message)
        {
            try
            {
                // Look for cache key.
                if (!_cache.TryGetValue(CacheKeys.Aapl, out List<Aapl> cacheEntry))
                {
                    // Key not in cache, so get data.
                    cacheEntry = ReadCsv("aapl.us.csv", ',');
                    // Set cache options.
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        // Keep in cache for this time, reset time if accessed.
                        .SetSlidingExpiration(TimeSpan.FromDays(3));
                    // Save data in cache.
                    _cache.Set(CacheKeys.Aapl, cacheEntry, cacheEntryOptions);
                }
                if (cacheEntry.Any())
                {
                    var sym = message.Msg.Substring(7);
                    var item = cacheEntry.SingleOrDefault(x => x.Symbol.Equals(sym, StringComparison.InvariantCultureIgnoreCase));
                    if (item == null)
                    {
                        return Ok(new { Success = true, From = "bot", Data = $"Hello {message.User}, I can't help you with {sym.ToUpper()}." });
                    }
                    //apply fx
                    var value = $"{item.Symbol.ToUpper()} quote is ${item.Open} per share.";
                    return Ok(new { Success = true, From = "bot", Data = value });
                }
                return Ok(null);
            }
            catch (Exception eX)
            {
                _logger.LogError(eX, eX.Message);
                return BadRequest(eX);
            }
        }

        private List<Aapl> ReadCsv(string fileName, char delimiter = ';')
        {
            var filePath = FileIO.Path.Combine(_env.WebRootPath + "\\docs\\", fileName);
            var lines = FileIO.File.ReadAllLines(filePath, Encoding.UTF8).Select(a => a.Split(delimiter)).ToArray();
            var aaplFile = new List<Aapl>();
            for (var i = 1; i < lines.Length; i++)
            {
                var item = new Aapl
                {
                    Symbol = lines[i][0],
                    Date = lines[i][1],
                    Time = lines[i][2],
                    Open = double.Parse(lines[i][3]),
                    High = double.Parse(lines[i][4]),
                    Low = double.Parse(lines[i][5]),
                    Close = double.Parse(lines[i][6]),
                    Volume = int.Parse(lines[i][7])
                };
                aaplFile.Add(item);
            }
            return (aaplFile);
        }
    }
}
