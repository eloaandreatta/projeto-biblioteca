using pBiblioteca.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace pBiblioteca.Controllers;

[ApiController]
[Route("[controller]")]
public class LoanController : ControllerBase
{
    
    private ILoanService _service;

    public LoanController(ILoanService service)
    {
        _service = service;
    }

    //GET LOAN
    [HttpGet(Name = "GetAllLoans")]
    public List<LoanResponseDTO> Get()
    {
        return _service.GetLoans();
    }

     // GET /Loan/{id}
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var loan = _service.GetLoanById(id);

        if (loan == null)
            return NotFound();

        return Ok(loan);
    }

    // POST /Loan
    [HttpPost]
    public IActionResult Post([FromBody] CreateLoanRequest request)
    {
        string error = _service.CreateLoan(request);

        if (error == "error")
            return BadRequest();

        return Ok();
    }

    // PUT /Loan/{id}/return
    [HttpPut("return/{id}")]
    public IActionResult ReturnLoan(int id)
    {
        string error = _service.ReturnLoan(id);

        if (error == "error")
            return BadRequest();

        return Ok();
    }

}