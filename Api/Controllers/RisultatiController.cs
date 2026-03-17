using Microsoft.AspNetCore.Mvc;
using WebApiBuddyGames.Application.Services.Interfaces;
using WebApiBuddyGames.Domain.Dto;

namespace WebApiBuddyGames.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RisultatiController(IRisultatoService risultatoService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await risultatoService.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await risultatoService.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRisultatoRequest request, CancellationToken cancellationToken)
    {
        var dto = new RisultatoDto
        {
            PartitaId = request.PartitaId,
            PuntiPrimaSquadra = request.PuntiPrimaSquadra,
            PuntiSecondaSquadra = request.PuntiSecondaSquadra,
            Attivo = true
        };

        var id = await risultatoService.CreateAsync(dto, cancellationToken);
        var created = await risultatoService.GetByIdAsync(id, cancellationToken);
        object response = created is null ? new { id } : created;

        return CreatedAtAction(nameof(GetById), new { id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRisultatoRequest request, CancellationToken cancellationToken)
    {
        var current = await risultatoService.GetByIdAsync(id, cancellationToken);
        if (current is null)
        {
            return NotFound();
        }

        var dto = new RisultatoDto
        {
            Id = id,
            PartitaId = request.PartitaId,
            PuntiPrimaSquadra = request.PuntiPrimaSquadra,
            PuntiSecondaSquadra = request.PuntiSecondaSquadra,
            Attivo = request.Attivo,
            DataCreazione = current.DataCreazione,
            DataCancellazione = current.DataCancellazione
        };

        var updated = await risultatoService.UpdateAsync(dto, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        var result = await risultatoService.GetByIdAsync(id, cancellationToken);
        object response = result is null ? new { id } : result;
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await risultatoService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    public sealed record CreateRisultatoRequest(int PartitaId, int PuntiPrimaSquadra, int PuntiSecondaSquadra);

    public sealed record UpdateRisultatoRequest(int PartitaId, int PuntiPrimaSquadra, int PuntiSecondaSquadra, bool Attivo = true);
}
