using WebApiBuddyGames.Domain.Dto;

namespace WebApiBuddyGames.Application.Services.Interfaces;

public interface IUtenteService
{
    Task<int> CreateAsync(UtenteDto dto, CancellationToken cancellationToken = default);

    Task<UtenteDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UtenteDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UtenteDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
