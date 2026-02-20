using pBiblioteca.Models;

public interface ILoanRepository
{
    public List<TbLoan> SelectLoans();

    public TbLoan? GetLoanById(int id);

    public List<TbLoan> GetLoansByUserCpf(string cpf);

    public bool InsertLoan(
        string userCpf,
        string bookIsbn,
        DateOnly loanDate,
        DateOnly dueDate
    );

    public bool UpdateLoan(
        int id,
        DateOnly? returnDate,
        bool status
    );
    public bool RenewLoan(int id, DateOnly newDueDate);
    public TbUser? GetUserByCpf(string cpf);
    public TbBook? GetBookByIsbn(string isbn);
    public bool UserHasActiveLoan(string cpf);
    public bool UserHasUnpaidFine(string cpf);
    void AddFine(TbFine fine);
    public void Save();
    List<LoanDetailsDTO> SelectLoanDetails(bool? status, string? userCpf, string? bookIsbn);

}
