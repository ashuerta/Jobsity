using System.Net.Http.Headers;
using jbx.core.Models.Rabbitmq;
using jbx.core.Models.Responses;
using jbx.infrastructure.Attributes;
using jbx.infrastructure.Rabbitmq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace jbx.ui.chat.Pages.Dashboard
{
    [JobsityAutorize]
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class DashboardModel : PageModel
    {
        HttpClient client = new HttpClient();
        private readonly ILogger<DashboardModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly AmqpService _amqp;

        public IConfiguration Configuration
        {
            get
            {
                return _configuration;
            }
        }

        public DashboardModel(
            ILogger<DashboardModel> logger,
            IConfiguration configuration,
            IWebHostEnvironment env,
            AmqpService amqp)
        {
            _logger = logger;
            _configuration = configuration;
            _env = env;
            _amqp = amqp ?? throw new ArgumentNullException(nameof(amqp));

            client.BaseAddress = new Uri(_configuration["Api:Chat"] ?? "https://localhost:8285/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostSendAsync(string user, string msg, DateTime date)
        {
            try
            {
                var response = new JobsityResponse()
                {
                    IsSuccess = false,
                    Message = "Cannot Connect to service!"
                };
                var model = new JobsityMessage()
                {
                    User = user,
                    Msg = msg,
                    Date = date
                };
                var r = _amqp.PublishMessage(model);
                var config = new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                if (r)
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {HttpContext.Session.GetString("jwtToken")}");
                    await Task.Delay(500);
                    var responseInsertMessage = await client.PostAsJsonAsync(
                    "api/message/AddMsg", model);
                    responseInsertMessage.EnsureSuccessStatusCode();
                    var res = await responseInsertMessage.Content.ReadFromJsonAsync<JobsityResponse>();
                    if (res!.IsSuccess)
                    {
                        response.IsSuccess = true;
                        response.Message = "Success";
                    }
                    return Content(JsonConvert.SerializeObject(response, Formatting.Indented, config));
                }
                return Content(JsonConvert.SerializeObject(response, Formatting.Indented, config));
            }
            catch (Exception eX)
            {
                return BadRequest(eX);
            }
        }
    }
}
