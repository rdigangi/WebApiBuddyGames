using Microsoft.EntityFrameworkCore;
using WebApiBuddyGames.Application.Repositories;
using WebApiBuddyGames.Domain.Dto;
using WebApiBuddyGames.Domain.Entities;
using WebApiBuddyGames.Infrastructure.Data;

namespace WebApiBuddyGames.Infrastructure.Repositories;

public class PartitaRepository(AppDbContext dbContext) : IPartitaRepository
{
    public async Task<int> CreateAsync(PartitaDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Partita
        {
            DataOraDisputa = dto.DataOraDisputa,
            Attivo = dto.Attivo,
            DataCancellazione = dto.DataCancellazione
        };

        await dbContext.Partite.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<PartitaDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Partite
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.DataCancellazione == null && x.Attivo, cancellationToken);

        return entity is null ? null : ToDto(entity);
    }

    public async Task<IReadOnlyList<PartitaDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Partite
            .AsNoTracking()
            .Where(x => x.DataCancellazione == null && x.Attivo)
            .OrderByDescending(x => x.DataOraDisputa)
            .Select(x => new PartitaDto
            {
                Id = x.Id,
                Attivo = x.Attivo,
                DataCreazione = x.DataCreazione,
                DataCancellazione = x.DataCancellazione,
                DataOraDisputa = x.DataOraDisputa
            })
            .ToListAsync(cancellationToken);

    public async Task<bool> UpdateAsync(PartitaDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Partite.FirstOrDefaultAsync(x => x.Id == dto.Id && x.DataCancellazione == null && x.Attivo, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.DataOraDisputa = dto.DataOraDisputa;
        entity.Attivo = dto.Attivo;
        entity.DataCancellazione = dto.DataCancellazione;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Partite.FirstOrDefaultAsync(x => x.Id == id && x.DataCancellazione == null && x.Attivo, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.Attivo = false;
        entity.DataCancellazione = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static PartitaDto ToDto(Partita entity) => new()
    {
        Id = entity.Id,
        Attivo = entity.Attivo,
        DataCreazione = entity.DataCreazione,
        DataCancellazione = entity.DataCancellazione,
        DataOraDisputa = entity.DataOraDisputa
    };
}
