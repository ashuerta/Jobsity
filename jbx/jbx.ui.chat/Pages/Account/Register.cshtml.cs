using System.Net.Http.Headers;
using jbx.core.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace jbx.ui.chat.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        HttpClient client = new HttpClient();
        private IConfiguration _configuration;

        public RegisterModel(IConfiguration configuration)
        {
            _configuration = configuration;
            // Update port # in the following line.
            client.BaseAddress = new Uri(_configuration["Api:Chat"] ?? "https://localhost:8285/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = "/Account/Login";

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostRegisterAsync(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await client.PostAsJsonAsync(
                    "api/auth/register", model);
                response.EnsureSuccessStatusCode();
                var token = response.Content;
                return new RedirectToPageResult("/Account/Login");
            }
            return new RedirectToPageResult("/Account/Register");
        }
    }
}
