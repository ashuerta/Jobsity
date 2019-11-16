using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Jobsity.Web.Models;
using Jobsity.Core.Entity;
using Microsoft.AspNetCore.Authorization;
using RestSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Jobsity.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IConfiguration config, IWebHostEnvironment env)
        {
            _logger = logger;
            _config = config;
            _env = env;
        }

        private ClaimsPrincipal GetPrincipalToken(string token)
        {
            //var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abraham.huerta@jobsity.com"));
            var t = token.Trim().Trim('"');
            var handler = new JwtSecurityTokenHandler();
            var tokenSecure = handler.ReadToken(t) as SecurityToken;
            var validations = new TokenValidationParameters
            {
                ValidateActor = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "http://ashuerta.com",
                ValidAudience = "http://ashuerta.com",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abraham.huerta@jobsity.com"))
            };
            var claims = handler.ValidateToken(t, validations, out tokenSecure);
            return claims;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> IndexAsync(JobsityUser entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    entity.Password = "Jobsity@123";
                    var client = new RestClient(_config["ApiConnection:Jobsity:Domain"]);
                    var request = new RestRequest("Security/SignIn/");
                    request.AddJsonBody(entity);
                    var response = client.Execute(request, Method.POST);
                    if (response == null || !response.IsSuccessful)
                    {
                        ModelState.AddModelError("General", "Can't login. Please contact to support team.");
                        return View();
                    }

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var jwToken = JsonConvert.DeserializeObject<JwToken>(response.Content);
                        //Save token in session object
                        HttpContext.Session.SetString("JWToken", jwToken.Token);
                        HttpContext.Session.SetString("JWExpiration", jwToken.Expiration.Ticks.ToString());
                        HttpContext.Session.SetString("sub", entity.UserName);
                        ///var token = UtilJwt.GenerateJwt(ud, jwt);
                        var decoded = GetPrincipalToken(jwToken.Token);
                        var identity = new ClaimsIdentity(decoded.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            principal,
                            new AuthenticationProperties
                            {
                                ExpiresUtc = DateTime.UtcNow.AddHours(12),
                                IsPersistent = true,
                                AllowRefresh = true,
                            }).ConfigureAwait(true);
                        return RedirectToAction("Index", "Chat");
                    }
                }
                ModelState.AddModelError("General", "Can't login. Please contact to support team.");
                return View();
            }
            catch (Exception eX)
            {
                Console.WriteLine(eX);
                return View();
            }
        }

        public ActionResult SignOut()
        {
            HttpContext.Session.Remove("JWToken");
            HttpContext.Session.Remove("JWExpiration");
            HttpContext.Session.Remove("sub");
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public IActionResult CreateOne()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult CreateOne([FromBody] JobsityUser entity)
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Created()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
