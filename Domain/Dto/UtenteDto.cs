namespace WebApiBuddyGames.Domain.Dto;

public class UtenteDto : EntityBaseDto
{
    public string Username { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string Cognome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? ProfileImageUrl { get; set; }

    public byte[] PasswordHash { get; set; } = [];

    public byte[] PasswordSalt { get; set; } = [];
}
