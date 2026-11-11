using HR.Application.DTOs.ProfileUpdates;
using HR.Application.Interfaces.Storage;
using HR.Application.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace HR.Infrastructure.Storage;

public sealed class LocalEmployeeUpdateEvidenceStore : IEmployeeUpdateEvidenceStore
{
    private const long MaxFileLength = 10 * 1024 * 1024;
    private readonly string _rootPath;

    public LocalEmployeeUpdateEvidenceStore(IConfiguration configuration, IWebHostEnvironment environment)
    {
        var configuredPath = configuration["Storage:EmployeeUpdateEvidencePath"];
        _rootPath = string.IsNullOrWhiteSpace(configuredPath)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HRM", "EmployeeUpdateEvidence")
            : Path.GetFullPath(configuredPath, environment.ContentRootPath);
    }

    public async Task<StoredEvidence> SaveAsync(EvidenceUpload upload, CancellationToken cancellationToken)
    {
        var fileName = NormalizeFileName(upload.FileName);
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var (contentType, signature) = extension switch
        {
            ".pdf" => ("application/pdf", new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }),
            ".jpg" or ".jpeg" => ("image/jpeg", new byte[] { 0xFF, 0xD8, 0xFF }),
            ".png" => ("image/png", new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
            _ => throw new HrException(400, "InvalidEvidenceType", "Chỉ chấp nhận file PDF, JPG, JPEG hoặc PNG.")
        };
        if (upload.Length <= 0 || upload.Length > MaxFileLength)
        {
            throw new HrException(400, "InvalidEvidenceSize", "File minh chứng phải lớn hơn 0 và không vượt quá 10 MB.");
        }

        await using var buffer = new MemoryStream();
        await upload.Content.CopyToAsync(buffer, cancellationToken);
        var bytes = buffer.ToArray();
        if (bytes.Length == 0 || bytes.Length > MaxFileLength || bytes.Length != upload.Length
            || !bytes.AsSpan().StartsWith(signature))
        {
            throw new HrException(400, "InvalidEvidenceContent", "Nội dung file minh chứng không hợp lệ.");
        }

        Directory.CreateDirectory(_rootPath);
        var storageKey = Guid.NewGuid().ToString("N") + extension;
        var path = GetPath(storageKey);
        try
        {
            await File.WriteAllBytesAsync(path, bytes, cancellationToken);
        }
        catch
        {
            TryDelete(path);
            throw;
        }
        return new StoredEvidence(storageKey, fileName, contentType, bytes.Length);
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        TryDelete(GetPath(storageKey));
        return Task.CompletedTask;
    }

    public Task<EvidenceDownload> OpenReadAsync(
        string storageKey,
        string originalFileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            Stream stream = new FileStream(GetPath(storageKey), FileMode.Open, FileAccess.Read, FileShare.Read);
            return Task.FromResult(new EvidenceDownload(stream, NormalizeFileName(originalFileName), contentType));
        }
        catch (FileNotFoundException)
        {
            throw new HrException(404, "EvidenceNotFound", "Không tìm thấy file minh chứng.");
        }
    }

    private string GetPath(string storageKey)
    {
        if (storageKey.Length is < 33 or > 37
            || storageKey[..32].Any(character => !Uri.IsHexDigit(character))
            || storageKey[32] != '.'
            || !new[] { ".pdf", ".jpg", ".jpeg", ".png" }.Contains(storageKey[32..], StringComparer.OrdinalIgnoreCase))
        {
            throw new HrException(400, "InvalidEvidenceKey", "Định danh file minh chứng không hợp lệ.");
        }
        return Path.Combine(_rootPath, storageKey);
    }

    private static string NormalizeFileName(string fileName)
    {
        var normalized = Path.GetFileName(fileName.Replace('\\', '/')).Trim();
        if (normalized.Length is 0 or > 255)
        {
            throw new HrException(400, "InvalidEvidenceName", "Tên file không hợp lệ hoặc vượt quá 255 ký tự.");
        }
        return normalized;
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // The caller logs the failed database operation; cleanup is best effort.
        }
    }
}
