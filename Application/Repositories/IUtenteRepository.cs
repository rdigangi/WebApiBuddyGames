using WebApiBuddyGames.Domain.Dto;

namespace WebApiBuddyGames.Application.Repositories;

public interface IUtenteRepository
{
    Task<int> CreateAsync(UtenteDto dto, CancellationToken cancellationToken = default);

    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<UtenteDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UtenteDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UtenteDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
