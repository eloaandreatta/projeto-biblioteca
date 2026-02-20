namespace pBiblioteca.Models;

public class LoanDetailsDTO
{
    public int LoanId { get; set; }

    public DateOnly LoanDate { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly? ReturnDate { get; set; }

    public string UserCpf { get; set; } = null!;
    public string UserName { get; set; } = null!;

    public string BookIsbn { get; set; } = null!;
    public string BookTitle { get; set; } = null!;
}