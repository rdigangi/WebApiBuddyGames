using Microsoft.AspNetCore.Mvc;
using WebApiBuddyGames.Application.Common;
using WebApiBuddyGames.Application.Services.Interfaces;
using WebApiBuddyGames.Domain.Dto;

namespace WebApiBuddyGames.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UtentiController(IUtenteService utenteService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var utenti = await utenteService.GetAllAsync(cancellationToken);
        return Ok(utenti.Select(ToResponse));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var utente = await utenteService.GetByIdAsync(id, cancellationToken);
        return utente is null ? NotFound() : Ok(ToResponse(utente));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUtenteRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return BadRequest(new { message = "Username obbligatorio." });
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Password obbligatoria." });
        }

        var (hash, salt) = PasswordHasher.HashPassword(request.Password);

        var dto = new UtenteDto
        {
            Username = request.Username,
            Nome = request.Nome,
            Cognome = request.Cognome,
            Email = request.Email,
            ProfileImageUrl = request.ProfileImageUrl,
            PasswordHash = hash,
            PasswordSalt = salt,
            Attivo = true
        };

        var id = await utenteService.CreateAsync(dto, cancellationToken);
        var created = await utenteService.GetByIdAsync(id, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, created is null ? new { id } : ToResponse(created));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUtenteRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return BadRequest(new { message = "Username obbligatorio." });
        }

        var current = await utenteService.GetByIdAsync(id, cancellationToken);
        if (current is null)
        {
            return NotFound();
        }

        var hash = current.PasswordHash;
        var salt = current.PasswordSalt;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            (hash, salt) = PasswordHasher.HashPassword(request.Password);
        }

        var dto = new UtenteDto
        {
            Id = id,
            Username = request.Username,
            Nome = request.Nome,
            Cognome = request.Cognome,
            Email = request.Email,
            ProfileImageUrl = request.ProfileImageUrl ?? current.ProfileImageUrl,
            PasswordHash = hash,
            PasswordSalt = salt,
            Attivo = request.Attivo,
            DataCreazione = current.DataCreazione,
            DataCancellazione = current.DataCancellazione
        };

        var updated = await utenteService.UpdateAsync(dto, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        var result = await utenteService.GetByIdAsync(id, cancellationToken);
        return Ok(result is null ? new { id } : ToResponse(result));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await utenteService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private static UtenteResponse ToResponse(UtenteDto dto) => new(
        dto.Id,
        dto.Username,
        dto.Nome,
        dto.Cognome,
        dto.Email,
        dto.ProfileImageUrl,
        dto.Attivo,
        dto.DataCreazione,
        dto.DataCancellazione);

    public sealed record CreateUtenteRequest(string Username, string Nome, string Cognome, string Email, string Password, string? ProfileImageUrl = null);

    public sealed record UpdateUtenteRequest(string Username, string Nome, string Cognome, string Email, string? Password, bool Attivo = true, string? ProfileImageUrl = null);

    public sealed record UtenteResponse(
        int Id,
        string Username,
        string Nome,
        string Cognome,
        string Email,
        string? ProfileImageUrl,
        bool Attivo,
        DateTime DataCreazione,
        DateTime? DataCancellazione);
}
