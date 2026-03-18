using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApiBuddyGames.Domain.Entities;

namespace WebApiBuddyGames.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Utente> Utenti => Set<Utente>();
    public DbSet<Ruolo> Ruoli => Set<Ruolo>();
    public DbSet<UtenteRuolo> UtentiRuoli => Set<UtenteRuolo>();
    public DbSet<Partita> Partite => Set<Partita>();
    public DbSet<PartitaUtente> PartiteUtenti => Set<PartitaUtente>();
    public DbSet<Risultato> Risultati => Set<Risultato>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Utente>(entity =>
        {
            entity.ToTable("utenti");
            ConfigureBaseEntity(entity);

            entity.Property(x => x.Username)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Nome)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Cognome)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Email)
                .HasMaxLength(255)
                .IsRequired();

            entity.HasIndex(x => x.Email)
                .IsUnique();

            entity.HasIndex(x => x.Username)
                .IsUnique();

            entity.Property(x => x.PasswordHash)
                .HasColumnType("bytea")
                .IsRequired();

            entity.Property(x => x.PasswordSalt)
                .HasColumnType("bytea")
                .IsRequired();
        });

        modelBuilder.Entity<Ruolo>(entity =>
        {
            entity.ToTable("ruoli");
            ConfigureBaseEntity(entity);

            entity.Property(x => x.Descrizione)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(x => x.Descrizione)
                .IsUnique();

            entity.HasData(
                new Ruolo
                {
                    Id = 1,
                    Descrizione = "Amministratore",
                    Attivo = true,
                    DataCreazione = new DateTime(2026, 3, 18, 0, 0, 0, DateTimeKind.Utc)
                },
                new Ruolo
                {
                    Id = 2,
                    Descrizione = "Utente",
                    Attivo = true,
                    DataCreazione = new DateTime(2026, 3, 18, 0, 0, 0, DateTimeKind.Utc)
                });
        });

        modelBuilder.Entity<UtenteRuolo>(entity =>
        {
            entity.ToTable("utenti_ruoli");
            ConfigureBaseEntity(entity);

            entity.HasOne(x => x.Utente)
                .WithMany(x => x.UtentiRuoli)
                .HasForeignKey(x => x.UtenteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Ruolo)
                .WithMany(x => x.UtentiRuoli)
                .HasForeignKey(x => x.RuoloId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.UtenteId, x.RuoloId })
                .IsUnique();
        });

        modelBuilder.Entity<Partita>(entity =>
        {
            entity.ToTable("partite");
            ConfigureBaseEntity(entity);

            entity.Property(x => x.DataOraDisputa)
                .HasDefaultValueSql("NOW()")
                .IsRequired();
        });

        modelBuilder.Entity<PartitaUtente>(entity =>
        {
            entity.ToTable("partite_utenti");
            ConfigureBaseEntity(entity);

            entity.HasOne(x => x.Partita)
                .WithMany(x => x.PartiteUtenti)
                .HasForeignKey(x => x.PartitaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Utente)
                .WithMany(x => x.PartiteUtenti)
                .HasForeignKey(x => x.UtenteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.PartitaId, x.UtenteId })
                .IsUnique();
        });

        modelBuilder.Entity<Risultato>(entity =>
        {
            entity.ToTable("risultati");
            ConfigureBaseEntity(entity);

            entity.Property(x => x.PuntiPrimaSquadra)
                .IsRequired();

            entity.Property(x => x.PuntiSecondaSquadra)
                .IsRequired();

            entity.HasOne(x => x.Partita)
                .WithOne(x => x.Risultato)
                .HasForeignKey<Risultato>(x => x.PartitaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.PartitaId)
                .IsUnique();
        });
    }

    private static void ConfigureBaseEntity<T>(EntityTypeBuilder<T> entity)
        where T : EntityBase
    {
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .UseIdentityByDefaultColumn()
            .HasIdentityOptions(startValue: 1, incrementBy: 1);

        entity.Property(x => x.Attivo)
            .HasDefaultValue(true)
            .IsRequired();

        entity.Property(x => x.DataCreazione)
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        entity.Property(x => x.DataCancellazione)
            .IsRequired(false);
    }
}
