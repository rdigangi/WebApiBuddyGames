using Microsoft.EntityFrameworkCore;
using WebApiBuddyGames.Application.Repositories;
using WebApiBuddyGames.Domain.Dto;
using WebApiBuddyGames.Domain.Entities;
using WebApiBuddyGames.Infrastructure.Data;

namespace WebApiBuddyGames.Infrastructure.Repositories;

public class UtenteRepository(AppDbContext dbContext) : IUtenteRepository
{
    public async Task<int> CreateAsync(UtenteDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Utente
        {
            Username = dto.Username,
            Nome = dto.Nome,
            Cognome = dto.Cognome,
            Email = dto.Email,
            PasswordHash = dto.PasswordHash,
            PasswordSalt = dto.PasswordSalt,
            Attivo = dto.Attivo,
            DataCancellazione = dto.DataCancellazione
        };

        await dbContext.Utenti.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        dbContext.Utenti.AnyAsync(
            x => x.Username == username && x.Attivo && x.DataCancellazione == null,
            cancellationToken);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        dbContext.Utenti.AnyAsync(
            x => x.Email == email && x.Attivo && x.DataCancellazione == null,
            cancellationToken);

    public async Task<UtenteDto?> GetByUsernameOrEmailAsync(string usernameOrEmail, CancellationToken cancellationToken = default)
    {
        var normalizedValue = usernameOrEmail.Trim().ToUpperInvariant();

        var entity = await dbContext.Utenti
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Attivo
                    && x.DataCancellazione == null
                    && (x.Username.ToUpper() == normalizedValue || x.Email.ToUpper() == normalizedValue),
                cancellationToken);

        return entity is null ? null : ToDto(entity);
    }

    public async Task<UtenteDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Utenti
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.DataCancellazione == null && x.Attivo, cancellationToken);

        return entity is null ? null : ToDto(entity);
    }

    public async Task<IReadOnlyList<UtenteDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Utenti
            .AsNoTracking()
            .Where(x => x.DataCancellazione == null && x.Attivo)
            .OrderBy(x => x.Cognome)
            .ThenBy(x => x.Nome)
            .Select(x => new UtenteDto
            {
                Id = x.Id,
                Attivo = x.Attivo,
                DataCreazione = x.DataCreazione,
                DataCancellazione = x.DataCancellazione,
                Username = x.Username,
                Nome = x.Nome,
                Cognome = x.Cognome,
                Email = x.Email,
                PasswordHash = x.PasswordHash,
                PasswordSalt = x.PasswordSalt
            })
            .ToListAsync(cancellationToken);

    public async Task<bool> UpdateAsync(UtenteDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Utenti.FirstOrDefaultAsync(x => x.Id == dto.Id && x.DataCancellazione == null && x.Attivo, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.Username = dto.Username;
        entity.Nome = dto.Nome;
        entity.Cognome = dto.Cognome;
        entity.Email = dto.Email;
        entity.PasswordHash = dto.PasswordHash;
        entity.PasswordSalt = dto.PasswordSalt;
        entity.Attivo = dto.Attivo;
        entity.DataCancellazione = dto.DataCancellazione;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Utenti.FirstOrDefaultAsync(x => x.Id == id && x.DataCancellazione == null && x.Attivo, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.Attivo = false;
        entity.DataCancellazione = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static UtenteDto ToDto(Utente entity) => new()
    {
        Id = entity.Id,
        Attivo = entity.Attivo,
        DataCreazione = entity.DataCreazione,
        DataCancellazione = entity.DataCancellazione,
        Username = entity.Username,
        Nome = entity.Nome,
        Cognome = entity.Cognome,
        Email = entity.Email,
        PasswordHash = entity.PasswordHash,
        PasswordSalt = entity.PasswordSalt
    };
}
