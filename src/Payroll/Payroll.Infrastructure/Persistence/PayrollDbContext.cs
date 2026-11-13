using Microsoft.EntityFrameworkCore;
using Payroll.Domain.Entities;
using PayrollEntity = global::Payroll.Domain.Entities.Payroll;
using Payroll.Application.Interfaces;

namespace Payroll.Infrastructure.Persistence;

public class PayrollDbContext : DbContext, IApplicationDbContext
{
    public PayrollDbContext (DbContextOptions<PayrollDbContext> options) : base(options)
    {
        
    }

    // Entities
    public DbSet<Allowance> Allowances {get; set;} = null!;
    public DbSet<AllowanceDetail> AllowanceDetails {get; set;} = null!;
    public DbSet<AllowanceType> AllowanceTypes {get; set;} = null!;

    public DbSet<PayCut> PayCuts {get; set;} = null!;
    public DbSet<PayCutDetail> PayCutDetails {get; set;} = null!;
    public DbSet<PayCutReason> PayCutReasons {get; set;} = null!;

    public DbSet<PayrollEntity> Payrolls {get; set;} = null!;
    public DbSet<PayrollPeriod> PayrollPeriods {get; set;} = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Allowance 1 - N AllowanceDetail
        // a = Allowance ad = AllowanceDetail at = AllowanceType
        modelBuilder.Entity<AllowanceDetail>()
        .HasOne(ad => ad.Allowance)
        .WithMany(a => a.AllowanceDetails)
        .HasForeignKey(ad => ad.AllowanceID);

        // AllowanceType 1 - N AllowanceDetail
        modelBuilder.Entity<AllowanceDetail>()
        .HasOne(ad => ad.AllowanceType)
        .WithMany(at => at.AllowanceDetails)
        .HasForeignKey(ad => ad.AllowanceTypeID);

        // PayrollPeriod 1 - N Payroll
        modelBuilder.Entity<PayrollEntity>()
        .HasOne(p => p.PayrollPeriod)
        .WithMany(pp => pp.Payrolls)
        .HasForeignKey(p => p.PayrollPeriodID);

        // Payroll 1 - N PayCut
        modelBuilder.Entity<PayCut>()
        .HasOne(pc => pc.Payroll)
        .WithMany(p => p.PayCuts)
        .HasForeignKey(pc => pc.PayrollID);

        // Payroll 1 - N Allowance
        modelBuilder.Entity<Allowance>()
        .HasOne(a => a.Payroll)
        .WithMany(p => p.Allowances)
        .HasForeignKey(a => a.PayrollID);

        // PayCut 1 - N PayCutDetail
        modelBuilder.Entity<PayCutDetail>()
        .HasOne(pd => pd.PayCut)
        .WithMany(pc => pc.Details)
        .HasForeignKey(pd => pd.PayCutID);

        // PayCutReason 1 - N PayCutDetail
        modelBuilder.Entity<PayCutDetail>()
        .HasOne(pd => pd.PayCutReason)
        .WithMany(pr => pr.PayCutDetails)
        .HasForeignKey(pd => pd.PayCutReasonID);

        // One employee can have only one payroll per payroll period
        modelBuilder.Entity<PayrollEntity>()
        .HasIndex(p => new { p.PayrollPeriodID, p.EmployeeID })
        .IsUnique();

        

        // Payroll monetary values
        // Payroll monetary values and properties
        modelBuilder.Entity<PayrollEntity>(entity =>
        {
            entity.Property(p => p.EmploymentType)
                .HasMaxLength(20);

            entity.Property(p => p.BaseSalary)
                .HasPrecision(18, 2);

            entity.Property(p => p.HourlyRate)
                .HasPrecision(18, 2);

            entity.Property(p => p.TotalAllowance)
                .HasPrecision(18, 2);

            entity.Property(p => p.TotalPayCut)
                .HasPrecision(18, 2);

            entity.Property(p => p.GrossSalary)
                .HasPrecision(18, 2);

            entity.Property(p => p.NetSalary)
                .HasPrecision(18, 2);

            entity.Property(p => p.WorkingDays)
                .HasPrecision(5, 2);

            entity.Property(p => p.StandardWorkingDay)
                .HasPrecision(5, 2);

            entity.Property(p => p.TotalWorkingHours)
                .HasPrecision(10, 2);
                
            entity.Property(p => p.OTHours)
                .HasPrecision(10, 2);

            entity.Property(p => p.OTMultiplier)
               .HasPrecision(5, 2);

            entity.Property(p => p.OTPay)
               .HasPrecision(18, 2);
        });

        // Allowance monetary values
        modelBuilder.Entity<AllowanceDetail>(entity =>
        {
            entity.Property(ad => ad.RateApplied).HasPrecision(18, 4);
            entity.Property(ad => ad.Amount).HasPrecision(18, 2);
        });

        // PayCut monetary values
        modelBuilder.Entity<PayCutDetail>(entity =>
        {
            entity.Property(pd => pd.RateApplied).HasPrecision(18, 4);
            entity.Property(pd => pd.Amount).HasPrecision(18, 2);
        });

        // Allowance configuration
        modelBuilder.Entity<AllowanceType>(entity =>
        {
            entity.Property(at => at.DefaultRate).HasPrecision(18, 4);
        });
        
        // Pay cut configuration
        modelBuilder.Entity<PayCutReason>(entity =>
        {
            entity.Property(pr => pr.DefaultRate).HasPrecision(18, 4);
        });

        modelBuilder.Entity<PayrollPeriod>()
        .HasIndex(pp => new { pp.Year, pp.Month})
        .IsUnique();
    }

}