using jbx.api.chat.Interfaces;
using jbx.core.Models.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace jbx.api.chat.Controllers
{
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        private readonly IMessageServices _service;

        public HomeController(IMessageServices service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("GetAllMsgs")]
        [Authorize]
        public async Task<IActionResult> GetMessagesAsync() => Ok(await Task.Run(() => _service.GetMessagesAsync()));

        [HttpPost]
        [Route("AddMsg")]
        [Authorize]
        public async Task<IActionResult> AddMessageAsync(MessageViewModel model) => Ok(await Task.Run(() => _service.AddMessageAsync(model)));
    }
}

