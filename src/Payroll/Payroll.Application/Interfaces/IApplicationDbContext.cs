using Microsoft.EntityFrameworkCore;
using Payroll.Domain.Entities;
using PayrollEntity = global::Payroll.Domain.Entities.Payroll;

namespace Payroll.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<PayrollPeriod> PayrollPeriods { get; }

    DbSet<PayrollEntity> Payrolls { get; }

    DbSet<Allowance> Allowances { get; }

    DbSet<AllowanceDetail> AllowanceDetails { get; }

    DbSet<AllowanceType> AllowanceTypes { get; }

    DbSet<PayCut> PayCuts { get; }

    DbSet<PayCutDetail> PayCutDetails { get; }

    DbSet<PayCutReason> PayCutReasons { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default
    );
}