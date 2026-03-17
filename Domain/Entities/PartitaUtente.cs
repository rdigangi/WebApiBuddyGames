namespace WebApiBuddyGames.Domain.Entities;

public class PartitaUtente : EntityBase
{
    public int PartitaId { get; set; }

    public int UtenteId { get; set; }

    public Partita Partita { get; set; } = null!;

    public Utente Utente { get; set; } = null!;
}
