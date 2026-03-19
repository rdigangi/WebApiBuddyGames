namespace WebApiBuddyGames.Application.Common;

public interface IProfileImageStorage
{
    Task<string> UploadAsync(string objectKey, Stream content, string contentType, CancellationToken cancellationToken = default);

    Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default);

    string? TryGetObjectKeyFromUrl(string? imageUrl);
}
