namespace WebApiBuddyGames.Domain.Entities;

public abstract class EntityBase
{
    public int Id { get; set; }

    public bool Attivo { get; set; } = true;

    public DateTime DataCreazione { get; set; }

    public DateTime? DataCancellazione { get; set; }
}
