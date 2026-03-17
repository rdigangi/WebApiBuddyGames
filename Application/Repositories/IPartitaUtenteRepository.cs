using WebApiBuddyGames.Domain.Dto;

namespace WebApiBuddyGames.Application.Repositories;

public interface IPartitaUtenteRepository
{
    Task<int> CreateAsync(PartitaUtenteDto dto, CancellationToken cancellationToken = default);

    Task<PartitaUtenteDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PartitaUtenteDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(PartitaUtenteDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
