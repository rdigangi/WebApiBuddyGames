using WebApiBuddyGames.Application.Common;

namespace WebApiBuddyGames.Application.Services.Interfaces;

public interface IProfileImageService
{
    Task<ServiceResult<string>> UploadAsync(
        int userId,
        string fileName,
        string contentType,
        long contentLength,
        Stream content,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> DeleteAsync(int userId, CancellationToken cancellationToken = default);
}
