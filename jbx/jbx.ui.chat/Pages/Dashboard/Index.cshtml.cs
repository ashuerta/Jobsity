using System.Net.Http.Headers;
using jbx.core.Models.Rabbitmq;
using jbx.core.Models.Responses;
using jbx.infrastructure.Attributes;
using jbx.infrastructure.Rabbitmq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using Formatting = Newtonsoft.Json.Formatting;
using Method = RestSharp.Method;

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
                new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
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
                    var client = new RestClient(_configuration["Api:Chat"] ?? "https://localhost:8285/");
                    var request = new RestRequest("api/message/AddMsg", Method.Post);
                    request.Parameters.RemoveParameter("Content-Type");
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("Authorization", $"Bearer {HttpContext.Session.GetString("jwtToken")}");
                    request.AddHeader("accept", "application/json");
                    request.RequestFormat = DataFormat.Json;
                    //request.AddBody(model);
                    request.AddObject(model);
                    //request.AddParameter("model", JsonConvert.SerializeObject(model), ParameterType.RequestBody);
                    
                    //var response = await client.ExecuteGetAsync(request);

                    //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {HttpContext.Session.GetString("jwtToken")}");
                    await Task.Delay(500);
                    //var responseInsertMessage = await client.PostAsJsonAsync(
                    //"api/message/AddMsg", JsonConvert.SerializeObject(model, Formatting.Indented));
                    //responseInsertMessage.EnsureSuccessStatusCode();
                    //var res = await responseInsertMessage.Content.ReadFromJsonAsync<JobsityResponse>();
                    var res = await client.ExecutePostAsync(request);
                    if (res.IsSuccessful)
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
