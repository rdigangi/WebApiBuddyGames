using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiBuddyGames.Application.Services.Interfaces;

namespace WebApiBuddyGames.Api.Controllers;

[ApiController]
[Route("me")]
[Produces("application/json")]
[Authorize]
public class MeController(
    IAuthenticationService authenticationService,
    IProfileImageService profileImageService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { message = "Token non valido." });
        }

        var result = await authenticationService.GetMeAsync(userId, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Error == "Utente non trovato.")
            {
                return NotFound(new { message = result.Error });
            }

            return Unauthorized(new { message = result.Error ?? "Accesso non autorizzato." });
        }

        return Ok(result.Value);
    }

    [HttpPost("profile-image")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadProfileImage([FromForm] UploadProfileImageRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { message = "Token non valido." });
        }

        if (request.File is null)
        {
            return BadRequest(new { message = "File immagine obbligatorio." });
        }

        await using var stream = request.File.OpenReadStream();
        var result = await profileImageService.UploadAsync(
            userId,
            request.File.FileName,
            request.File.ContentType,
            request.File.Length,
            stream,
            cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Utente non trovato.")
            {
                return NotFound(new { message = result.Error });
            }

            return BadRequest(new { message = result.Error ?? "Upload immagine non riuscito." });
        }

        return Ok(new
        {
            profileImageUrl = result.Value,
            message = "Immagine profilo aggiornata con successo."
        });
    }

    [HttpDelete("profile-image")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProfileImage(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { message = "Token non valido." });
        }

        var result = await profileImageService.DeleteAsync(userId, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Error == "Utente non trovato.")
            {
                return NotFound(new { message = result.Error });
            }

            return BadRequest(new { message = result.Error ?? "Cancellazione immagine non riuscita." });
        }

        return Ok(new { message = "Immagine profilo rimossa con successo." });
    }

    private bool TryGetCurrentUserId(out int userId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return int.TryParse(userIdClaim, out userId);
    }

    public sealed record UploadProfileImageRequest(IFormFile? File);
}
