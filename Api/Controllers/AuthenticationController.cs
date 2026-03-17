using Microsoft.AspNetCore.Mvc;
using WebApiBuddyGames.Application.Services.Interfaces;
using WebApiBuddyGames.Domain.Dto.RequestDto;

namespace WebApiBuddyGames.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
    {
        var result = await authenticationService.RegisterNewUser(request, cancellationToken);
        if (!result.IsSuccess || result.Value <= 0)
        {
            return BadRequest(new
            {
                userId = 0,
                message = result.Error ?? "Registrazione non riuscita."
            });
        }
        var id = result.Value;
        return Created($"/api/utenti/{id}", new
        {
            userId = id,
            message = "Utente registrato con successo."
        });
    }
}
