namespace WebApiBuddyGames.Domain.Entities;

public class Risultato : EntityBase
{
    public int PartitaId { get; set; }

    public int PuntiPrimaSquadra { get; set; }

    public int PuntiSecondaSquadra { get; set; }

    public Partita Partita { get; set; } = null!;
}
