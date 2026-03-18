namespace WebApiBuddyGames.Domain.Entities;

public class Ruolo : EntityBase
{
    public string Descrizione { get; set; } = string.Empty;

    public ICollection<UtenteRuolo> UtentiRuoli { get; set; } = new List<UtenteRuolo>();
}
