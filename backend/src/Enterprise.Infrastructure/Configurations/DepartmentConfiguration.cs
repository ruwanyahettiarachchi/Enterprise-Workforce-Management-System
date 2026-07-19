using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Enterprise.Domain.Entities;

namespace Enterprise.Infrastructure.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(d => d.Name)
            .IsUnique();

        builder.Property(d => d.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(d => d.Code)
            .IsUnique();

        // Optional relationship for Department Manager linking to Employees
        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(d => d.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(d => d.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(d => d.LastModifiedAt)
            .IsRequired(false);
    }
}
