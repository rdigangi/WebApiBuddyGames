using WebApiBuddyGames.Domain.Dto;

namespace WebApiBuddyGames.Application.Repositories;

public interface IPartitaRepository
{
    Task<int> CreateAsync(PartitaDto dto, CancellationToken cancellationToken = default);

    Task<PartitaDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PartitaDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(PartitaDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
