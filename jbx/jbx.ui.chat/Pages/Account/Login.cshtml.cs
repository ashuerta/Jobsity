using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using jbx.core.Entities.Security;
using jbx.core.Models.Identity;
using jbx.core.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace jbx.ui.chat.Pages.Account
{
	public class LoginModel : PageModel
    {
        HttpClient client = new HttpClient();
        private IConfiguration _configuration;

        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; } = "/Login";

        public LoginModel(IConfiguration configuration)
        {
            _configuration = configuration;
            // Update port # in the following line.
            client.BaseAddress = new Uri(_configuration["Api:Chat"] ?? "https://localhost:8285/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostLoginAsync(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await client.PostAsJsonAsync(
                    "api/auth/login", model);
                response.EnsureSuccessStatusCode();
                var token = await response.Content.ReadFromJsonAsync<JobsityResponse>();
                if (token?.Message != null)
                {
                    HttpContext.Session.SetString("jwtToken", token.Message);
                    HttpContext.Session.SetString("sub", model.Username);
                }
                return new RedirectToPageResult("/Dashboard/Index");
            }
            return new RedirectToPageResult("/Account/Login");
        }
    }
}
