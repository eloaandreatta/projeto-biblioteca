using pBiblioteca.Models;

public interface ILoanRepository
{
    public List<TbLoan> SelectLoans();

    public TbLoan? GetLoanById(int id);

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

    public TbUser? GetUserByCpf(string cpf);
    public TbBook? GetBookByIsbn(string isbn);
    public bool UserHasActiveLoan(string cpf);
    public bool UserHasUnpaidFine(string cpf);

    void AddFine(TbFine fine);

    public void Save();

}
