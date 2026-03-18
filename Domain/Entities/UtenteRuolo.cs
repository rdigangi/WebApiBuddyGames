namespace WebApiBuddyGames.Domain.Entities;

public class UtenteRuolo : EntityBase
{
    public int UtenteId { get; set; }

    public int RuoloId { get; set; }

    public Utente Utente { get; set; } = null!;

    public Ruolo Ruolo { get; set; } = null!;
}
