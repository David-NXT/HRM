using HR.Domain.Entities;

namespace HR.Application.Interfaces.Services;

public interface IContractAlertGenerator
{
    Task GenerateForContractAsync(EmploymentContract contract, CancellationToken cancellationToken);
}
