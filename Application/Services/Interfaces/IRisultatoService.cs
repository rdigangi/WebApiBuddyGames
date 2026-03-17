using WebApiBuddyGames.Domain.Dto;

namespace WebApiBuddyGames.Application.Services.Interfaces;

public interface IRisultatoService
{
    Task<int> CreateAsync(RisultatoDto dto, CancellationToken cancellationToken = default);

    Task<RisultatoDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RisultatoDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(RisultatoDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
