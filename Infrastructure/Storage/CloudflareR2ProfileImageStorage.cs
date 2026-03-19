using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using WebApiBuddyGames.Application.Common;
using WebApiBuddyGames.Infrastructure.Options;

namespace WebApiBuddyGames.Infrastructure.Storage;

public class CloudflareR2ProfileImageStorage(
    IAmazonS3 s3Client,
    IOptions<CloudflareR2Options> options) : IProfileImageStorage
{
    private readonly IAmazonS3 _s3Client = s3Client;
    private readonly CloudflareR2Options _options = options.Value;

    public async Task<string> UploadAsync(string objectKey, Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = objectKey,
            InputStream = content,
            AutoCloseStream = false,
            ContentType = contentType,
            UseChunkEncoding = false,
            DisablePayloadSigning = true
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);

        var baseUrl = ResolvePublicBaseUrl();
        return $"{baseUrl}/{EncodeObjectKey(objectKey)}";
    }

    public async Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _options.BucketName,
            Key = objectKey
        };

        await _s3Client.DeleteObjectAsync(request, cancellationToken);
    }

    public string? TryGetObjectKeyFromUrl(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return null;
        }

        if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var imageUri))
        {
            return null;
        }

        var baseUrl = ResolvePublicBaseUrl();
        if (Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri)
            && string.Equals(baseUri.Host, imageUri.Host, StringComparison.OrdinalIgnoreCase))
        {
            var basePath = baseUri.AbsolutePath.TrimEnd('/');
            var imagePath = imageUri.AbsolutePath;

            if (!string.IsNullOrWhiteSpace(basePath) && imagePath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
            {
                imagePath = imagePath[basePath.Length..];
            }

            return Uri.UnescapeDataString(imagePath.TrimStart('/'));
        }

        return null;
    }

    private string ResolvePublicBaseUrl()
    {
        if (!string.IsNullOrWhiteSpace(_options.PublicBaseUrl))
        {
            return _options.PublicBaseUrl.TrimEnd('/');
        }

        var serviceUrl = ResolveServiceUrl().TrimEnd('/');
        return $"{serviceUrl}/{_options.BucketName}";
    }

    private string ResolveServiceUrl()
    {
        if (!string.IsNullOrWhiteSpace(_options.ServiceUrl))
        {
            return _options.ServiceUrl;
        }

        if (string.IsNullOrWhiteSpace(_options.AccountId))
        {
            throw new InvalidOperationException("CloudflareR2: AccountId non configurato.");
        }

        return $"https://{_options.AccountId}.r2.cloudflarestorage.com";
    }

    private static string EncodeObjectKey(string objectKey) =>
        string.Join('/', objectKey.Split('/', StringSplitOptions.RemoveEmptyEntries).Select(Uri.EscapeDataString));
}
