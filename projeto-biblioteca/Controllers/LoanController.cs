using pBiblioteca.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace pBiblioteca.Controllers;

[ApiController]
[Route("[controller]")]
public class LoanController : ControllerBase
{
    private readonly ILoanService _service;
    private readonly IWebHostEnvironment _env;

    public LoanController(ILoanService service, IWebHostEnvironment env)
    {
        _service = service;
        _env = env;
    }

   [HttpGet]
    public IActionResult Get(
        [FromQuery] bool? status,
        [FromQuery] string? userCpf,
        [FromQuery] string? bookIsbn)
    {
        var loans = _service.GetLoans(status, userCpf, bookIsbn);
        return Ok(loans);
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

    // GET /Loan/report
    [HttpGet("report")]
    public IActionResult GenerateLoanReport()
    {
        var loans = _service.GetLoans(null, null, null);

        if (loans == null || !loans.Any())
            return NotFound("Nenhum empréstimo encontrado.");

        var linhas = new List<string>();

        linhas.Add("LoanId;UserCpf;BookIsbn;LoanDate;DueDate;ReturnDate;Status;DiasAtraso");

        foreach (var loan in loans)
        {
            int diasAtraso = 0;

            if (loan.ReturnDate == null &&
                loan.DueDate < DateOnly.FromDateTime(DateTime.Today))
            {
                diasAtraso =
                    DateOnly.FromDateTime(DateTime.Today)
                    .DayNumber - loan.DueDate.DayNumber;
            }

            var status = loan.ReturnDate == null
                ? (diasAtraso > 0 ? "Atrasado" : "Ativo")
                : "Devolvido";

            linhas.Add($"{loan.Id};{loan.UserCpf};{loan.BookIsbn};" +
                       $"{loan.LoanDate};{loan.DueDate};{loan.ReturnDate};" +
                       $"{status};{diasAtraso}");
        }

        var pasta = Path.Combine(_env.ContentRootPath, "Relatorios");

        if (!Directory.Exists(pasta))
            Directory.CreateDirectory(pasta);

        var nomeArquivo = $"relatorio-loans-{DateTime.Now:yyyyMMdd-HHmmss}.csv";
        var caminho = Path.Combine(pasta, nomeArquivo);

        System.IO.File.WriteAllLines(caminho, linhas);

        return File(System.IO.File.ReadAllBytes(caminho),
                    "text/csv",
                    nomeArquivo);
    }

    [HttpGet("details")]
public IActionResult GetDetails(
    [FromQuery] bool? status,
    [FromQuery] string? userCpf,
    [FromQuery] string? bookIsbn)
{
    var result = _service.GetLoanDetails(status, userCpf, bookIsbn);

    if (result == null || !result.Any())
        return NotFound();

    return Ok(result);
}

}