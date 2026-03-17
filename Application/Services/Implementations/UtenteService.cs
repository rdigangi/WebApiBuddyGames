using WebApiBuddyGames.Application.Repositories;
using WebApiBuddyGames.Application.Services.Interfaces;
using WebApiBuddyGames.Domain.Dto;

namespace WebApiBuddyGames.Application.Services.Implementations;

public class UtenteService(IUtenteRepository repository) : IUtenteService
{
    public Task<int> CreateAsync(UtenteDto dto, CancellationToken cancellationToken = default) =>
        repository.CreateAsync(dto, cancellationToken);

    public Task<UtenteDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        repository.GetByIdAsync(id, cancellationToken);

    public Task<IReadOnlyList<UtenteDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetAllAsync(cancellationToken);

    public Task<bool> UpdateAsync(UtenteDto dto, CancellationToken cancellationToken = default) =>
        repository.UpdateAsync(dto, cancellationToken);

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        repository.DeleteAsync(id, cancellationToken);
}
