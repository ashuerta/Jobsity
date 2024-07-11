using jbx.core.Entities.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace jbx.infrastructure.Mappings
{
	public class MessageMapping : IEntityTypeConfiguration<Message>
	{
		public void Configure(EntityTypeBuilder<Message> builder)
		{
			builder.HasKey(b => b.Id);

			builder.Property(b => b.Consumer)
				.HasMaxLength(150)
				.IsRequired();

			builder.Property(b => b.Sender)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(b => b.TypedMessage)
                .HasMaxLength(255)
                .IsRequired();
        }
	}
}

