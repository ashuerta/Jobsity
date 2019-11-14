using Jobsity.Core.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Jobsity.Data
{
    public class SecurityRepository : ExternalRepository<JobsityUser, JobsityContext>
    {
        public SecurityRepository(JobsityContext ctx)
        {
            _ctx = ctx;
        }
    }
}
