using WebApiBuddyGames.Application.Repositories;
using WebApiBuddyGames.Application.Services.Interfaces;
using WebApiBuddyGames.Domain.Dto;

namespace WebApiBuddyGames.Application.Services.Implementations;

public class PartitaUtenteService(IPartitaUtenteRepository repository) : IPartitaUtenteService
{
    public Task<int> CreateAsync(PartitaUtenteDto dto, CancellationToken cancellationToken = default) =>
        repository.CreateAsync(dto, cancellationToken);

    public Task<PartitaUtenteDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        repository.GetByIdAsync(id, cancellationToken);

    public Task<IReadOnlyList<PartitaUtenteDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetAllAsync(cancellationToken);

    public Task<bool> UpdateAsync(PartitaUtenteDto dto, CancellationToken cancellationToken = default) =>
        repository.UpdateAsync(dto, cancellationToken);

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        repository.DeleteAsync(id, cancellationToken);
}
