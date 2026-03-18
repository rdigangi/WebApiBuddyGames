using WebApiBuddyGames.Domain.Dto;

namespace WebApiBuddyGames.Application.Repositories;

public interface IUtenteRuoloRepository
{
    Task<int> CreateAsync(UtenteRuoloDto dto, CancellationToken cancellationToken = default);

    Task<UtenteRuoloDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRoleDescriptionsByUserIdAsync(int utenteId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UtenteRuoloDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UtenteRuoloDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
