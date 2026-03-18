namespace WebApiBuddyGames.Domain.Entities;

public class RefreshToken : EntityBase
{
    public int UtenteId { get; set; }

    public string TokenHash { get; set; } = string.Empty;

    public DateTime ScadenzaUtc { get; set; }

    public DateTime? RevocatoUtc { get; set; }

    public Utente Utente { get; set; } = null!;
}
