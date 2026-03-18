using WebApiBuddyGames.Application.Common;
using WebApiBuddyGames.Domain.Dto;
using WebApiBuddyGames.Domain.Dto.RequestDto;

namespace WebApiBuddyGames.Application.Services.Interfaces;

public interface IAuthenticationService
{
    Task<ServiceResult<int>> RegisterNewUser(RegisterRequestDto request, CancellationToken cancellationToken = default);

    Task<ServiceResult<AuthTokensDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

    Task<ServiceResult<AuthTokensDto>> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default);
}
