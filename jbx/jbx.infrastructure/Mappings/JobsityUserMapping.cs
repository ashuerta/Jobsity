using jbx.core.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace jbx.infrastructure.Mappings
{
	public class JobsityUserMapping : IEntityTypeConfiguration<JobsityUser>
    {
        public void Configure(EntityTypeBuilder<JobsityUser> builder)
        {
            builder.Property(b => b.FirstName)
                .HasMaxLength(255);

            builder.Property(b => b.LastName)
                .HasMaxLength(255);
        }
    }
}

