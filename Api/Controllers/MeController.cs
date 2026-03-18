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
public class MeController(IAuthenticationService authenticationService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!int.TryParse(userIdClaim, out var userId))
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
}
