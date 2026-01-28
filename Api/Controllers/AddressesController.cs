using Application.Dtos.Addresses;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressesController : Controller
    {
        private readonly IAddressService _service;

        public AddressesController(IAddressService service)
        {
            _service = service;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AddressResponse>> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var result = await _service.GetByIdAsync(id, ct);

            if (result is null)
                return NotFound(new { message = "Endereço não encontrado." });

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<AddressResponse>> Create([FromBody] CreateAddressRequest request, CancellationToken ct)
        {
            try
            {
                var created = await _service.CreateAsync(request, ct);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Ex: CEP inválido no provedor, CEP não encontrado, etc.
                return UnprocessableEntity(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<AddressResponse>> Update([FromRoute] Guid id, [FromBody] UpdateAddressRequest request, CancellationToken ct)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, request, ct);

                if (updated is null)
                    return NotFound(new { message = "Endereço não encontrado." });

                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return UnprocessableEntity(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
        {
            var deleted = await _service.DeleteAsync(id, ct);

            if (!deleted)
                return NotFound(new { message = "Endereço não encontrado." });

            return NoContent();
        }
    }
}
