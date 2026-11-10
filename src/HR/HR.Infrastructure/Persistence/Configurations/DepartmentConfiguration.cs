using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("department");
        builder.HasKey(department => department.DepartmentId);
        builder.Property(department => department.DepartmentId).HasColumnName("department_id").HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();
        builder.Property(department => department.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
        builder.HasIndex(department => department.Code).IsUnique();
        builder.Property(department => department.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(department => department.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(department => department.ParentDepartmentId).HasColumnName("parent_department_id");
        builder.Property(department => department.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(department => department.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(department => department.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.HasOne(department => department.ParentDepartment)
            .WithMany(department => department.ChildDepartments)
            .HasForeignKey(department => department.ParentDepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
