using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Jobsity.Core.Entity;
using Jobsity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Jobsity.Service.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class SecurityController : ControllerBase
    {
        private readonly ILogger<SecurityController> _logger;
        private readonly SecurityRepository _repository;
        private readonly UserManager<JobsityUser> _manager;
        
        public SecurityController(ILogger<SecurityController> logger, SecurityRepository repository, UserManager<JobsityUser> manager)
        {
            _logger = logger;
            _repository = repository;
            _manager = manager;
        }

        [HttpPost]
        [Route("SignIn")]
        public async Task<IActionResult> LoginAsync([FromBody] JobsityUser model)
        {
            try
            {
                var expireTicks = DateTime.UtcNow.AddHours(3);
                var user = await _manager.FindByNameAsync(model.UserName);
                if (user != null && await _manager.CheckPasswordAsync(user, model.Password))
                {
                    var authClaims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim("expireTicks", expireTicks.Ticks.ToString())
                    };

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abraham.huerta@jobsity.com"));

                    var token = new JwtSecurityToken(
                        issuer: "http://ashuerta.com",
                        audience: "http://ashuerta.com",
                        expires: expireTicks,
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                        );

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }
                return Unauthorized();
            }
            catch (Exception eX)
            {
                _logger.LogError(eX, eX.Message);
                return BadRequest(eX);
            }
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] JobsityUser model)
        {
            try
            {
                model.SecurityStamp = Guid.NewGuid().ToString();
                var created = await _manager.CreateAsync(model, model.Password);
                
                if (created != null)
                {
                    return Ok(new
                    {
                        Success = created.Succeeded,
                        Data = created,
                        Message = created.Errors.Any() ? created.Errors.FirstOrDefault().Description : string.Empty
                    });
                }

                return Unauthorized();
            }
            catch (Exception eX)
            {
                _logger.LogError(eX, eX.Message);
                return BadRequest(eX);
            }
        }

        [HttpGet]
        [Route("AllUsers")]
        [Authorize]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var r = await _repository.GetAll();
                
                if (r.Any())
                {
                    return Ok(r);    
                }
                return Ok(null);
            }
            catch (Exception eX)
            {
                _logger.LogError(eX, eX.Message);
                return BadRequest(eX);
            } 
        }
    }
}
