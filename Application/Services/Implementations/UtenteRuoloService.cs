using WebApiBuddyGames.Application.Repositories;
using WebApiBuddyGames.Application.Services.Interfaces;
using WebApiBuddyGames.Domain.Dto;

namespace WebApiBuddyGames.Application.Services.Implementations;

public class UtenteRuoloService(IUtenteRuoloRepository repository) : IUtenteRuoloService
{
    public Task<int> CreateAsync(UtenteRuoloDto dto, CancellationToken cancellationToken = default) =>
        repository.CreateAsync(dto, cancellationToken);

    public Task<UtenteRuoloDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        repository.GetByIdAsync(id, cancellationToken);

    public Task<IReadOnlyList<UtenteRuoloDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetAllAsync(cancellationToken);

    public Task<bool> UpdateAsync(UtenteRuoloDto dto, CancellationToken cancellationToken = default) =>
        repository.UpdateAsync(dto, cancellationToken);

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        repository.DeleteAsync(id, cancellationToken);
}
