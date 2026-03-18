using WebApiBuddyGames.Application.Repositories;
using WebApiBuddyGames.Application.Services.Interfaces;
using WebApiBuddyGames.Domain.Dto;

namespace WebApiBuddyGames.Application.Services.Implementations;

public class RuoloService(IRuoloRepository repository) : IRuoloService
{
    public Task<int> CreateAsync(RuoloDto dto, CancellationToken cancellationToken = default) =>
        repository.CreateAsync(dto, cancellationToken);

    public Task<RuoloDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        repository.GetByIdAsync(id, cancellationToken);

    public Task<IReadOnlyList<RuoloDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetAllAsync(cancellationToken);

    public Task<bool> UpdateAsync(RuoloDto dto, CancellationToken cancellationToken = default) =>
        repository.UpdateAsync(dto, cancellationToken);

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        repository.DeleteAsync(id, cancellationToken);
}
