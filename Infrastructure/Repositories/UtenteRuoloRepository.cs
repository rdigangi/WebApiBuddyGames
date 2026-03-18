using Microsoft.EntityFrameworkCore;
using WebApiBuddyGames.Application.Repositories;
using WebApiBuddyGames.Domain.Dto;
using WebApiBuddyGames.Domain.Entities;
using WebApiBuddyGames.Infrastructure.Data;

namespace WebApiBuddyGames.Infrastructure.Repositories;

public class UtenteRuoloRepository(AppDbContext dbContext) : IUtenteRuoloRepository
{
    public async Task<int> CreateAsync(UtenteRuoloDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new UtenteRuolo
        {
            UtenteId = dto.UtenteId,
            RuoloId = dto.RuoloId,
            Attivo = dto.Attivo,
            DataCancellazione = dto.DataCancellazione
        };

        await dbContext.UtentiRuoli.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<UtenteRuoloDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.UtentiRuoli
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.DataCancellazione == null && x.Attivo, cancellationToken);

        return entity is null ? null : ToDto(entity);
    }

    public async Task<IReadOnlyList<string>> GetRoleDescriptionsByUserIdAsync(int utenteId, CancellationToken cancellationToken = default) =>
        await dbContext.UtentiRuoli
            .AsNoTracking()
            .Where(x => x.UtenteId == utenteId && x.Attivo && x.DataCancellazione == null)
            .Where(x => x.Ruolo.Attivo && x.Ruolo.DataCancellazione == null)
            .Select(x => x.Ruolo.Descrizione)
            .Distinct()
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<UtenteRuoloDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.UtentiRuoli
            .AsNoTracking()
            .Where(x => x.DataCancellazione == null && x.Attivo)
            .OrderByDescending(x => x.Id)
            .Select(x => new UtenteRuoloDto
            {
                Id = x.Id,
                Attivo = x.Attivo,
                DataCreazione = x.DataCreazione,
                DataCancellazione = x.DataCancellazione,
                UtenteId = x.UtenteId,
                RuoloId = x.RuoloId
            })
            .ToListAsync(cancellationToken);

    public async Task<bool> UpdateAsync(UtenteRuoloDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.UtentiRuoli.FirstOrDefaultAsync(x => x.Id == dto.Id && x.DataCancellazione == null && x.Attivo, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.UtenteId = dto.UtenteId;
        entity.RuoloId = dto.RuoloId;
        entity.Attivo = dto.Attivo;
        entity.DataCancellazione = dto.DataCancellazione;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.UtentiRuoli.FirstOrDefaultAsync(x => x.Id == id && x.DataCancellazione == null && x.Attivo, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.Attivo = false;
        entity.DataCancellazione = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static UtenteRuoloDto ToDto(UtenteRuolo entity) => new()
    {
        Id = entity.Id,
        Attivo = entity.Attivo,
        DataCreazione = entity.DataCreazione,
        DataCancellazione = entity.DataCancellazione,
        UtenteId = entity.UtenteId,
        RuoloId = entity.RuoloId
    };
}
