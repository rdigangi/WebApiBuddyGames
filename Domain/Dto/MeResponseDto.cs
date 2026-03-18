namespace WebApiBuddyGames.Domain.Dto;

public sealed class MeResponseDto
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string Cognome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public IReadOnlyList<string> Ruoli { get; set; } = [];
}