using Jobsity.Core.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Jobsity.Data
{
    public class JobsityContext : IdentityDbContext<JobsityUser>
    {
        public JobsityContext(DbContextOptions<JobsityContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
