using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

namespace Jobsity.Web.Controllers
{
    public class JobsityController : Controller
    {
        protected readonly IConfiguration _config;
        protected readonly IWebHostEnvironment _env;

        public JobsityController(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }
    }
}
