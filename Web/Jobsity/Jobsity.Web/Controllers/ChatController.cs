using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Jobsity.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using Jobsity.Core.Entity;

namespace Jobsity.Web.Controllers
{
    public class ChatController : Controller
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly AmqpService _amqp;

        public ChatController(ILogger<ChatController> logger, IConfiguration config, IWebHostEnvironment env, AmqpService amqp)
        {
            _logger = logger;
            _config = config;
            _env = env;
            _amqp = amqp ?? throw new ArgumentNullException(nameof(amqp));
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("Chat/SendMsg")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PublishMessage([FromBody] JobsityMessage message)
        {
            try
            {
                var r = _amqp.PublishMessage(message);
                if (r)
                {
                    return Ok(new
                    {
                        Success = true,
                        Data = string.Empty,
                        Message = "Success"
                    });
                }
                return Ok(new
                {
                    Success = false,
                    Data = string.Empty,
                    Message = "Cannot Connect to service!"
                });
            }
            catch (Exception eX)
            {
                return BadRequest(eX);
            }
            
            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
