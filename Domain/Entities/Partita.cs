namespace WebApiBuddyGames.Domain.Entities;

public class Partita : EntityBase
{
    public DateTime DataOraDisputa { get; set; }

    public ICollection<PartitaUtente> PartiteUtenti { get; set; } = new List<PartitaUtente>();

    public Risultato? Risultato { get; set; }
}
