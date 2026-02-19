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
    public IActionResult Get()
    {
        return Ok(_service.GetLoans());
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

    // GET /Loan/user/{cpf}
    [HttpGet("user/{cpf}")]
    public IActionResult GetByUser(string cpf)
    {
        var loans = _service.GetLoansByUser(cpf);

        if (loans == null || !loans.Any())
            return NotFound();

        return Ok(loans);
    }

    // POST /Loan
    [HttpPost]
    public IActionResult Post([FromBody] CreateLoanRequest request)
    {
        string error = _service.CreateLoan(request);

        if (error == "error")
            return BadRequest();

        return Created("", null);

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

    // PUT /Loan/renew/{id}
    [HttpPut("renew/{id}")]
    public IActionResult RenewLoan(int id)
    {
        string result = _service.RenewLoan(id);

        if (result == "error")
            return BadRequest();

        return Ok();
    }

}