using Microsoft.EntityFrameworkCore;
using WebApiBuddyGames.Application.Repositories;
using WebApiBuddyGames.Domain.Dto;
using WebApiBuddyGames.Domain.Entities;
using WebApiBuddyGames.Infrastructure.Data;

namespace WebApiBuddyGames.Infrastructure.Repositories;

public class RefreshTokenRepository(AppDbContext dbContext) : IRefreshTokenRepository
{
    public async Task<int> CreateAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new RefreshToken
        {
            UtenteId = dto.UtenteId,
            TokenHash = dto.TokenHash,
            ScadenzaUtc = dto.ScadenzaUtc,
            RevocatoUtc = dto.RevocatoUtc,
            Attivo = dto.Attivo,
            DataCancellazione = dto.DataCancellazione
        };

        await dbContext.RefreshTokens.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<RefreshTokenDto?> GetActiveByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var entity = await dbContext.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.TokenHash == tokenHash
                && x.Attivo
                && x.DataCancellazione == null
                && x.RevocatoUtc == null
                && x.ScadenzaUtc > now,
                cancellationToken);

        return entity is null ? null : ToDto(entity);
    }

    public async Task<bool> RevokeAsync(int id, DateTime revokedAtUtc, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.Id == id && x.DataCancellazione == null && x.Attivo, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        entity.RevocatoUtc = revokedAtUtc;
        entity.Attivo = false;
        entity.DataCancellazione = revokedAtUtc;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static RefreshTokenDto ToDto(RefreshToken entity) => new()
    {
        Id = entity.Id,
        Attivo = entity.Attivo,
        DataCreazione = entity.DataCreazione,
        DataCancellazione = entity.DataCancellazione,
        UtenteId = entity.UtenteId,
        TokenHash = entity.TokenHash,
        ScadenzaUtc = entity.ScadenzaUtc,
        RevocatoUtc = entity.RevocatoUtc
    };
}
