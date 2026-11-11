using HR.Application.DTOs.ProfileUpdates;
namespace HR.Application.Interfaces.Storage;

public interface IEmployeeUpdateEvidenceStore
{
    Task<StoredEvidence> SaveAsync(EvidenceUpload upload, CancellationToken cancellationToken);
    Task DeleteAsync(string storageKey, CancellationToken cancellationToken);
    Task<EvidenceDownload> OpenReadAsync(string storageKey, string originalFileName, string contentType, CancellationToken cancellationToken);
}
