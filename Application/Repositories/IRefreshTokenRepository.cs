using WebApiBuddyGames.Domain.Dto;

namespace WebApiBuddyGames.Application.Repositories;

public interface IRefreshTokenRepository
{
    Task<int> CreateAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default);

    Task<RefreshTokenDto?> GetActiveByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task<bool> RevokeAsync(int id, DateTime revokedAtUtc, CancellationToken cancellationToken = default);
}
