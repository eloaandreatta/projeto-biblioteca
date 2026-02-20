using Microsoft.AspNetCore.Mvc;
using pBiblioteca.DTO;

namespace pBiblioteca.Controllers;

[ApiController]
[Route("[controller]")]
public class FineController : ControllerBase
{
    private readonly IFineService _service;

    public FineController(IFineService service)
    {
        _service = service;
    }

    // ✅ Usuário pode consultar Multa (por CPF)
    [HttpGet("user/{cpf}")]
    public ActionResult<List<FineResponseDTO>> GetByCpf(string cpf)
    {
        var fines = _service.GetFinesByCpf(cpf);
        return Ok(fines);
    }

    // ✅ Consulta multa por empréstimo (cria/atualiza se necessário)
    [HttpGet("loan/{loanId:int}")]
    public ActionResult<FineResponseDTO> GetByLoan(int loanId)
    {
        var fine = _service.GetOrCreateFineByLoanId(loanId);
        if (fine == null) return NotFound();

        return Ok(fine);
    }

    // ✅ Usuário pode pagar multa
    [HttpPost("{id:int}/pay")]
    public IActionResult Pay(int id)
    {
        var result = _service.PayFine(id);
        if (result == "error") return BadRequest("error");

        return Ok();
    }
}
