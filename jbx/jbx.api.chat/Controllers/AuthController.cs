using jbx.api.chat.Interfaces;
using jbx.core.Models.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace jbx.api.chat.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        public IUserServices _userServices;

        public AuthController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        // /api/auth/login 
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userServices.LoginUserAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Some properties are not valid");
        }

        // /api/auth/register 
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userServices.RegisterUserAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result); //Status code: 200
                }
                return BadRequest(result); //Status code: 400
            }
            return BadRequest("Some properties are not valid"); //Status code: 400
        }

    }
}

