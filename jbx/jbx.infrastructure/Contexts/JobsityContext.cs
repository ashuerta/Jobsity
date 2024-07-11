using jbx.core.Entities.Messages;
using jbx.core.Entities.Security;
using jbx.infrastructure.Mappings;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace jbx.infrastructure.Contexts
{
	public class JobsityContext : IdentityDbContext<JobsityUser>
    {
		//public virtual DbSet<JobsityUser> Users { get; set; }
        public virtual DbSet<Message> Messages { get; set; }

        public JobsityContext(DbContextOptions<JobsityContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new JobsityUserMapping());
            builder.ApplyConfiguration(new MessageMapping());
            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}

