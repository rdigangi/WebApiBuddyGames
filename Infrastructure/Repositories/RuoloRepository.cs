using Microsoft.EntityFrameworkCore;
using WebApiBuddyGames.Application.Repositories;
using WebApiBuddyGames.Domain.Dto;
using WebApiBuddyGames.Domain.Entities;
using WebApiBuddyGames.Infrastructure.Data;

namespace WebApiBuddyGames.Infrastructure.Repositories;

public class RuoloRepository(AppDbContext dbContext) : IRuoloRepository
{
    public async Task<int> CreateAsync(RuoloDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Ruolo
        {
            Descrizione = dto.Descrizione,
            Attivo = dto.Attivo,
            DataCancellazione = dto.DataCancellazione
        };

        await dbContext.Ruoli.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<RuoloDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Ruoli
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.DataCancellazione == null && x.Attivo, cancellationToken);

        return entity is null ? null : ToDto(entity);
    }

    public async Task<RuoloDto?> GetByDescrizioneAsync(string descrizione, CancellationToken cancellationToken = default)
    {
        var normalizedDescrizione = descrizione.Trim().ToUpperInvariant();
        var entity = await dbContext.Ruoli
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Attivo
                    && x.DataCancellazione == null
                    && x.Descrizione.ToUpper() == normalizedDescrizione,
                cancellationToken);

        return entity is null ? null : ToDto(entity);
    }

    public async Task<IReadOnlyList<RuoloDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Ruoli
            .AsNoTracking()
            .Where(x => x.DataCancellazione == null && x.Attivo)
            .OrderBy(x => x.Descrizione)
            .Select(x => new RuoloDto
            {
                Id = x.Id,
                Attivo = x.Attivo,
                DataCreazione = x.DataCreazione,
                DataCancellazione = x.DataCancellazione,
                Descrizione = x.Descrizione
            })
            .ToListAsync(cancellationToken);

    public async Task<bool> UpdateAsync(RuoloDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Ruoli.FirstOrDefaultAsync(x => x.Id == dto.Id && x.DataCancellazione == null && x.Attivo, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.Descrizione = dto.Descrizione;
        entity.Attivo = dto.Attivo;
        entity.DataCancellazione = dto.DataCancellazione;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Ruoli.FirstOrDefaultAsync(x => x.Id == id && x.DataCancellazione == null && x.Attivo, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.Attivo = false;
        entity.DataCancellazione = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static RuoloDto ToDto(Ruolo entity) => new()
    {
        Id = entity.Id,
        Attivo = entity.Attivo,
        DataCreazione = entity.DataCreazione,
        DataCancellazione = entity.DataCancellazione,
        Descrizione = entity.Descrizione
    };
}
