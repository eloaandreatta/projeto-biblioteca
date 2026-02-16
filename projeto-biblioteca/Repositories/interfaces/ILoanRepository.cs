using pBiblioteca.Models;

public interface ILoanRepository
{
    List<TbLoan> SelectLoans();

    TbLoan? GetLoanById(int id);

    bool InsertLoan(
        string userCpf,
        string bookIsbn,
        DateOnly loanDate,
        DateOnly dueDate
    );

    bool UpdateLoan(
        int id,
        DateOnly? returnDate,
        bool status
    );
}
