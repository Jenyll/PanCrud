using Application.Dtos.Persons;
using Application.Dtos.Persons.Request;
using Application.Dtos.Persons.Response;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonsController : Controller
{
    private readonly IPersonService _service;

    public PersonsController(IPersonService service) => _service = service;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PersonResponse>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        if (result is null) return NotFound(new { message = "Pessoa não encontrada." });
        return Ok(result);
    }

    [HttpGet("document")]
    public async Task<ActionResult<PersonResponse>> GetByDocument(
    [FromQuery] string number,
    [FromQuery] PersonType type, // 0 PF, 1 PJ (ou Individual/Company)
    CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(number))
            return BadRequest(new { message = "number é obrigatório." });

        var result = await _service.GetByDocumentAsync(number, type, ct);

        if (result is null)
            return NotFound(new { message = "Pessoa não encontrada." });

        return Ok(result);
    }


    [HttpPost]
    public async Task<ActionResult<PersonResponse>> Create([FromBody] CreatePersonRequest request, CancellationToken ct)
    {
        try
        {
            var created = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetByDocument), new { number = request.Type == PersonType.Individual ? request.Cpf : request.Cnpj, type = request.Type }, created);
        }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return UnprocessableEntity(new { message = ex.Message }); }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PersonResponse>> Update([FromRoute] Guid id, [FromBody] UpdatePersonRequest request, CancellationToken ct)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, request, ct);
            if (updated is null) return NotFound(new { message = "Pessoa não encontrada." });
            return Ok(updated);
        }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return UnprocessableEntity(new { message = ex.Message }); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var deleted = await _service.DeleteAsync(id, ct);
        if (!deleted) return NotFound(new { message = "Pessoa não encontrada." });
        return NoContent();
    }
}