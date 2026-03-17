using Microsoft.EntityFrameworkCore;
using WebApiBuddyGames.Application.Repositories;
using WebApiBuddyGames.Domain.Dto;
using WebApiBuddyGames.Domain.Entities;
using WebApiBuddyGames.Infrastructure.Data;

namespace WebApiBuddyGames.Infrastructure.Repositories;

public class PartitaUtenteRepository(AppDbContext dbContext) : IPartitaUtenteRepository
{
    public async Task<int> CreateAsync(PartitaUtenteDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new PartitaUtente
        {
            PartitaId = dto.PartitaId,
            UtenteId = dto.UtenteId,
            Attivo = dto.Attivo,
            DataCancellazione = dto.DataCancellazione
        };

        await dbContext.PartiteUtenti.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<PartitaUtenteDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.PartiteUtenti
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.DataCancellazione == null && x.Attivo, cancellationToken);

        return entity is null ? null : ToDto(entity);
    }

    public async Task<IReadOnlyList<PartitaUtenteDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.PartiteUtenti
            .AsNoTracking()
            .Where(x => x.DataCancellazione == null && x.Attivo)
            .OrderByDescending(x => x.Id)
            .Select(x => new PartitaUtenteDto
            {
                Id = x.Id,
                Attivo = x.Attivo,
                DataCreazione = x.DataCreazione,
                DataCancellazione = x.DataCancellazione,
                PartitaId = x.PartitaId,
                UtenteId = x.UtenteId
            })
            .ToListAsync(cancellationToken);

    public async Task<bool> UpdateAsync(PartitaUtenteDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.PartiteUtenti.FirstOrDefaultAsync(x => x.Id == dto.Id && x.DataCancellazione == null && x.Attivo, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.PartitaId = dto.PartitaId;
        entity.UtenteId = dto.UtenteId;
        entity.Attivo = dto.Attivo;
        entity.DataCancellazione = dto.DataCancellazione;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.PartiteUtenti.FirstOrDefaultAsync(x => x.Id == id && x.DataCancellazione == null && x.Attivo, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.Attivo = false;
        entity.DataCancellazione = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static PartitaUtenteDto ToDto(PartitaUtente entity) => new()
    {
        Id = entity.Id,
        Attivo = entity.Attivo,
        DataCreazione = entity.DataCreazione,
        DataCancellazione = entity.DataCancellazione,
        PartitaId = entity.PartitaId,
        UtenteId = entity.UtenteId
    };
}
