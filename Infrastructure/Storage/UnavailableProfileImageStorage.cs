using WebApiBuddyGames.Application.Common;

namespace WebApiBuddyGames.Infrastructure.Storage;

public class UnavailableProfileImageStorage : IProfileImageStorage
{
    private const string ErrorMessage = "Cloudflare R2 non configurato. Imposta i valori in 'CloudflareR2' (AccountId/ServiceUrl, AccessKeyId, SecretAccessKey, BucketName).";

    public Task<string> UploadAsync(string objectKey, Stream content, string contentType, CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException(ErrorMessage);

    public Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException(ErrorMessage);

    public string? TryGetObjectKeyFromUrl(string? imageUrl) => null;
}
