using WebApiBuddyGames.Application.Repositories;
using WebApiBuddyGames.Application.Services.Interfaces;
using WebApiBuddyGames.Domain.Dto;

namespace WebApiBuddyGames.Application.Services.Implementations;

public class RisultatoService(IRisultatoRepository repository) : IRisultatoService
{
    public Task<int> CreateAsync(RisultatoDto dto, CancellationToken cancellationToken = default) =>
        repository.CreateAsync(dto, cancellationToken);

    public Task<RisultatoDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        repository.GetByIdAsync(id, cancellationToken);

    public Task<IReadOnlyList<RisultatoDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetAllAsync(cancellationToken);

    public Task<bool> UpdateAsync(RisultatoDto dto, CancellationToken cancellationToken = default) =>
        repository.UpdateAsync(dto, cancellationToken);

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        repository.DeleteAsync(id, cancellationToken);
}
