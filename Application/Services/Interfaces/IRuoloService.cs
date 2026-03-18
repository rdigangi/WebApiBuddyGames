using WebApiBuddyGames.Domain.Dto;

namespace WebApiBuddyGames.Application.Services.Interfaces;

public interface IRuoloService
{
    Task<int> CreateAsync(RuoloDto dto, CancellationToken cancellationToken = default);

    Task<RuoloDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RuoloDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(RuoloDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
