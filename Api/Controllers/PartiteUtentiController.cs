using Microsoft.AspNetCore.Mvc;
using WebApiBuddyGames.Application.Services.Interfaces;
using WebApiBuddyGames.Domain.Dto;

namespace WebApiBuddyGames.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PartiteUtentiController(IPartitaUtenteService partitaUtenteService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await partitaUtenteService.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await partitaUtenteService.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePartitaUtenteRequest request, CancellationToken cancellationToken)
    {
        var dto = new PartitaUtenteDto
        {
            PartitaId = request.PartitaId,
            UtenteId = request.UtenteId,
            Attivo = true
        };

        var id = await partitaUtenteService.CreateAsync(dto, cancellationToken);
        var created = await partitaUtenteService.GetByIdAsync(id, cancellationToken);
        object response = created is null ? new { id } : created;

        return CreatedAtAction(nameof(GetById), new { id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePartitaUtenteRequest request, CancellationToken cancellationToken)
    {
        var current = await partitaUtenteService.GetByIdAsync(id, cancellationToken);
        if (current is null)
        {
            return NotFound();
        }

        var dto = new PartitaUtenteDto
        {
            Id = id,
            PartitaId = request.PartitaId,
            UtenteId = request.UtenteId,
            Attivo = request.Attivo,
            DataCreazione = current.DataCreazione,
            DataCancellazione = current.DataCancellazione
        };

        var updated = await partitaUtenteService.UpdateAsync(dto, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        var result = await partitaUtenteService.GetByIdAsync(id, cancellationToken);
        object response = result is null ? new { id } : result;
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await partitaUtenteService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    public sealed record CreatePartitaUtenteRequest(int PartitaId, int UtenteId);

    public sealed record UpdatePartitaUtenteRequest(int PartitaId, int UtenteId, bool Attivo = true);
}
