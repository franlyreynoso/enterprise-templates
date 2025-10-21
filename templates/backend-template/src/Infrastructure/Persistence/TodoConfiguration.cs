using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseTemplate.Application.Todos;

namespace EnterpriseTemplate.Infrastructure.Persistence;

public sealed class TodoConfiguration : IEntityTypeConfiguration<Todo>
{
    public void Configure(EntityTypeBuilder<Todo> b)
    {
        b.ToTable("todos");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.Title).IsRequired().HasMaxLength(200);
        b.Property(x => x.IsDone).HasDefaultValue(false);
        b.Property(x => x.OwnerId).IsRequired().HasMaxLength(100);

        // Audit properties
        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.CreatedBy).IsRequired().HasMaxLength(100);
        b.Property(x => x.UpdatedAt).IsRequired();
        b.Property(x => x.UpdatedBy).IsRequired().HasMaxLength(100);

        // Soft delete properties
        b.Property(x => x.DeletedAt);
        b.Property(x => x.DeletedBy).HasMaxLength(100);

        // Indexes
        b.HasIndex(x => new { x.OwnerId, x.IsDone });
        b.HasIndex(x => x.CreatedAt);
        b.HasIndex(x => x.DeletedAt);
    }
}
