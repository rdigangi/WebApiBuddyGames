using Amazon.S3;
using WebApiBuddyGames.Application.Common;
using WebApiBuddyGames.Application.Repositories;
using WebApiBuddyGames.Application.Services.Interfaces;

namespace WebApiBuddyGames.Application.Services.Implementations;

public class ProfileImageService(
    IUtenteRepository utenteRepository,
    IProfileImageStorage profileImageStorage) : IProfileImageService
{
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    ];

    private static readonly Dictionary<string, string> ContentTypeToExtension = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/jpeg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/webp"] = ".webp",
        ["image/gif"] = ".gif"
    };

    private static readonly Dictionary<string, string> ExtensionToContentType = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".webp"] = "image/webp",
        [".gif"] = "image/gif"
    };

    public async Task<ServiceResult<string>> UploadAsync(
        int userId,
        string fileName,
        string contentType,
        long contentLength,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
        {
            return ServiceResult<string>.Failure("Utente non valido.");
        }

        if (string.IsNullOrWhiteSpace(fileName) || contentLength <= 0)
        {
            return ServiceResult<string>.Failure("File immagine non valido.");
        }

        if (contentLength > MaxFileSizeBytes)
        {
            return ServiceResult<string>.Failure("Dimensione massima immagine: 5 MB.");
        }

        var normalizedContentType = NormalizeContentType(contentType, fileName);
        if (!AllowedContentTypes.Contains(normalizedContentType))
        {
            return ServiceResult<string>.Failure("Formato immagine non supportato.");
        }

        var user = await utenteRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return ServiceResult<string>.Failure("Utente non trovato.");
        }

        var extension = GetExtension(fileName, normalizedContentType);
        var objectKey = $"users/{userId}/profile/{Guid.NewGuid():N}{extension}";

        string uploadedUrl;

        try
        {
            uploadedUrl = await profileImageStorage.UploadAsync(objectKey, content, normalizedContentType, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return ServiceResult<string>.Failure(ex.Message);
        }
        catch (AmazonS3Exception)
        {
            return ServiceResult<string>.Failure("Upload immagine non riuscito: verifica credenziali Cloudflare R2 e permessi bucket.");
        }
        catch
        {
            return ServiceResult<string>.Failure("Upload immagine non riuscito.");
        }

        var updated = await utenteRepository.UpdateProfileImageUrlAsync(userId, uploadedUrl, cancellationToken);
        if (!updated)
        {
            try
            {
                await profileImageStorage.DeleteAsync(objectKey, cancellationToken);
            }
            catch
            {
                // Best effort cleanup.
            }

            return ServiceResult<string>.Failure("Impossibile salvare l'immagine profilo per l'utente.");
        }

        await TryDeleteOldImageAsync(user.ProfileImageUrl, objectKey, cancellationToken);

        return ServiceResult<string>.Success(uploadedUrl);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int userId, CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
        {
            return ServiceResult<bool>.Failure("Utente non valido.");
        }

        var user = await utenteRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return ServiceResult<bool>.Failure("Utente non trovato.");
        }

        if (!string.IsNullOrWhiteSpace(user.ProfileImageUrl))
        {
            var currentObjectKey = profileImageStorage.TryGetObjectKeyFromUrl(user.ProfileImageUrl);
            if (!string.IsNullOrWhiteSpace(currentObjectKey))
            {
                try
                {
                    await profileImageStorage.DeleteAsync(currentObjectKey, cancellationToken);
                }
                catch
                {
                    return ServiceResult<bool>.Failure("Cancellazione immagine dal bucket non riuscita.");
                }
            }
        }

        var updated = await utenteRepository.UpdateProfileImageUrlAsync(userId, null, cancellationToken);
        if (!updated)
        {
            return ServiceResult<bool>.Failure("Impossibile aggiornare il profilo utente.");
        }

        return ServiceResult<bool>.Success(true);
    }

    private async Task TryDeleteOldImageAsync(string? previousImageUrl, string currentObjectKey, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(previousImageUrl))
        {
            return;
        }

        var previousObjectKey = profileImageStorage.TryGetObjectKeyFromUrl(previousImageUrl);
        if (string.IsNullOrWhiteSpace(previousObjectKey) || previousObjectKey == currentObjectKey)
        {
            return;
        }

        try
        {
            await profileImageStorage.DeleteAsync(previousObjectKey, cancellationToken);
        }
        catch
        {
            // Best effort cleanup.
        }
    }

    private static string GetExtension(string fileName, string contentType)
    {
        var extension = Path.GetExtension(fileName)?.Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(extension) && extension.StartsWith('.'))
        {
            return extension;
        }

        if (ContentTypeToExtension.TryGetValue(contentType, out var mappedExtension))
        {
            return mappedExtension;
        }

        return ".bin";
    }

    private static string NormalizeContentType(string contentType, string fileName)
    {
        if (!string.IsNullOrWhiteSpace(contentType)
            && !string.Equals(contentType, "application/octet-stream", StringComparison.OrdinalIgnoreCase))
        {
            return contentType.Trim().ToLowerInvariant();
        }

        var extension = Path.GetExtension(fileName)?.Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(extension) && ExtensionToContentType.TryGetValue(extension, out var mappedContentType))
        {
            return mappedContentType;
        }

        return contentType?.Trim().ToLowerInvariant() ?? string.Empty;
    }
}
