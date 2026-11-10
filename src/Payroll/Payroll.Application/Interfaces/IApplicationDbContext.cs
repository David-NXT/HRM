using Payroll.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Payroll.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<PayrollPeriod> PayrollPeriods {get;}

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default
    );
}