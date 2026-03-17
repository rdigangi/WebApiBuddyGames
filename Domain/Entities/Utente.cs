namespace WebApiBuddyGames.Domain.Entities;

public class Utente : EntityBase
{
    public string Username { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string Cognome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    // Non memorizzare mai password in chiaro.
    public byte[] PasswordHash { get; set; } = [];

    public byte[] PasswordSalt { get; set; } = [];

    public ICollection<PartitaUtente> PartiteUtenti { get; set; } = new List<PartitaUtente>();
}
