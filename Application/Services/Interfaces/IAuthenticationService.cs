using WebApiBuddyGames.Application.Common;
using WebApiBuddyGames.Domain.Dto.RequestDto;

namespace WebApiBuddyGames.Application.Services.Interfaces;

public interface IAuthenticationService
{
    Task<ServiceResult<int>> RegisterNewUser(RegisterRequestDto request, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}
