using Microsoft.EntityFrameworkCore;
using WebApiBuddyGames.Application.Repositories;
using WebApiBuddyGames.Domain.Dto;
using WebApiBuddyGames.Domain.Entities;
using WebApiBuddyGames.Infrastructure.Data;

namespace WebApiBuddyGames.Infrastructure.Repositories;

public class RisultatoRepository(AppDbContext dbContext) : IRisultatoRepository
{
    public async Task<int> CreateAsync(RisultatoDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Risultato
        {
            PartitaId = dto.PartitaId,
            PuntiPrimaSquadra = dto.PuntiPrimaSquadra,
            PuntiSecondaSquadra = dto.PuntiSecondaSquadra,
            Attivo = dto.Attivo,
            DataCancellazione = dto.DataCancellazione
        };

        await dbContext.Risultati.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<RisultatoDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Risultati
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.DataCancellazione == null && x.Attivo, cancellationToken);

        return entity is null ? null : ToDto(entity);
    }

    public async Task<IReadOnlyList<RisultatoDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Risultati
            .AsNoTracking()
            .Where(x => x.DataCancellazione == null && x.Attivo)
            .OrderByDescending(x => x.Id)
            .Select(x => new RisultatoDto
            {
                Id = x.Id,
                Attivo = x.Attivo,
                DataCreazione = x.DataCreazione,
                DataCancellazione = x.DataCancellazione,
                PartitaId = x.PartitaId,
                PuntiPrimaSquadra = x.PuntiPrimaSquadra,
                PuntiSecondaSquadra = x.PuntiSecondaSquadra
            })
            .ToListAsync(cancellationToken);

    public async Task<bool> UpdateAsync(RisultatoDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Risultati.FirstOrDefaultAsync(x => x.Id == dto.Id && x.DataCancellazione == null && x.Attivo, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.PartitaId = dto.PartitaId;
        entity.PuntiPrimaSquadra = dto.PuntiPrimaSquadra;
        entity.PuntiSecondaSquadra = dto.PuntiSecondaSquadra;
        entity.Attivo = dto.Attivo;
        entity.DataCancellazione = dto.DataCancellazione;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Risultati.FirstOrDefaultAsync(x => x.Id == id && x.DataCancellazione == null && x.Attivo, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.Attivo = false;
        entity.DataCancellazione = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static RisultatoDto ToDto(Risultato entity) => new()
    {
        Id = entity.Id,
        Attivo = entity.Attivo,
        DataCreazione = entity.DataCreazione,
        DataCancellazione = entity.DataCancellazione,
        PartitaId = entity.PartitaId,
        PuntiPrimaSquadra = entity.PuntiPrimaSquadra,
        PuntiSecondaSquadra = entity.PuntiSecondaSquadra
    };
}
