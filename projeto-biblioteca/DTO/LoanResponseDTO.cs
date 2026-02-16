namespace pBiblioteca.Models;

public class LoanResponseDTO
{
    public int Id { get; set; }
    public string UserCpf { get; set; } = null!;
    public string BookIsbn { get; set; } = null!;

    public DateOnly LoanDate { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly? ReturnDate { get; set; }

    public bool Status { get; set; }
}


public class CreateLoanRequest
{
    public string UserCpf { get; set; } = null!;

    public string BookIsbn { get; set; } = null!;
    public DateTime DueDate { get; set; }
}

