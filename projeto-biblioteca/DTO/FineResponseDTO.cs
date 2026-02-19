namespace pBiblioteca.DTO;

public class FineResponseDTO
{
    public int Id { get; set; }
    public string UserCpf { get; set; } = null!;
    public int LoanId { get; set; }

    public decimal Amount { get; set; }
    public int DaysLate { get; set; }

    public bool IsPaid { get; set; }
    public decimal DailyRate { get; set; }

    public DateOnly PaymentDate { get; set; }
}
