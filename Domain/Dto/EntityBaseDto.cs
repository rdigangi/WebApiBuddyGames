namespace WebApiBuddyGames.Domain.Dto;

public class EntityBaseDto
{
    public int Id { get; set; }

    public bool Attivo { get; set; }

    public DateTime DataCreazione { get; set; }

    public DateTime? DataCancellazione { get; set; }
}
