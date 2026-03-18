using WebApiBuddyGames.Domain.Dto;

namespace WebApiBuddyGames.Application.Services.Interfaces;

public interface IUtenteRuoloService
{
    Task<int> CreateAsync(UtenteRuoloDto dto, CancellationToken cancellationToken = default);

    Task<UtenteRuoloDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UtenteRuoloDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UtenteRuoloDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
