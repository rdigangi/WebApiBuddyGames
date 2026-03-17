using Microsoft.AspNetCore.Mvc;
using WebApiBuddyGames.Application.Services.Interfaces;
using WebApiBuddyGames.Domain.Dto;

namespace WebApiBuddyGames.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PartiteController(IPartitaService partitaService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await partitaService.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await partitaService.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePartitaRequest request, CancellationToken cancellationToken)
    {
        var dto = new PartitaDto
        {
            DataOraDisputa = request.DataOraDisputa,
            Attivo = true
        };

        var id = await partitaService.CreateAsync(dto, cancellationToken);
        var created = await partitaService.GetByIdAsync(id, cancellationToken);
        object response = created is null ? new { id } : created;

        return CreatedAtAction(nameof(GetById), new { id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePartitaRequest request, CancellationToken cancellationToken)
    {
        var current = await partitaService.GetByIdAsync(id, cancellationToken);
        if (current is null)
        {
            return NotFound();
        }

        var dto = new PartitaDto
        {
            Id = id,
            DataOraDisputa = request.DataOraDisputa,
            Attivo = request.Attivo,
            DataCreazione = current.DataCreazione,
            DataCancellazione = current.DataCancellazione
        };

        var updated = await partitaService.UpdateAsync(dto, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        var result = await partitaService.GetByIdAsync(id, cancellationToken);
        object response = result is null ? new { id } : result;
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await partitaService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    public sealed record CreatePartitaRequest(DateTime DataOraDisputa);

    public sealed record UpdatePartitaRequest(DateTime DataOraDisputa, bool Attivo = true);
}
