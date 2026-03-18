namespace WebApiBuddyGames.Domain.Dto;

public sealed class RefreshTokenDto : EntityBaseDto
{
    public int UtenteId { get; set; }

    public string TokenHash { get; set; } = string.Empty;

    public DateTime ScadenzaUtc { get; set; }

    public DateTime? RevocatoUtc { get; set; }
}
