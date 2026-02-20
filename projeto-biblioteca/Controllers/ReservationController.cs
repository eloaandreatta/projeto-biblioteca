using pBiblioteca.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pBiblioteca.Services;

namespace pBiblioteca.Controllers;

// Responsavel apenas por receber e enviar dados pro cliente
// Conecta ao Service
[ApiController]
[Route("[controller]")]
public class ReservationController : ControllerBase
{
    private readonly IReservationService _service;

    public ReservationController(IReservationService service)
    {
        _service = service;
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateReservationRequest request)
    {
        var result = _service.CreateReservation(request);

        if (result != "ok") return BadRequest(result);
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Cancel([FromRoute] int id)
    {
        var result = _service.CancelReservation(id);

        if (result == "not_found") return NotFound();
        if (result != "ok") return BadRequest(result);

        return Ok();
    }

    [HttpGet("queue/{isbn}")]
    public IActionResult GetQueue([FromRoute] string isbn)
        => Ok(_service.GetQueue(isbn));

    [HttpGet("position/{isbn}/{cpf}")]
    public IActionResult GetPosition([FromRoute] string isbn, [FromRoute] string cpf)
    {
        var pos = _service.GetUserPosition(isbn, cpf);
        if (pos == -1) return NotFound();
        return Ok(new { position = pos });
    }
}