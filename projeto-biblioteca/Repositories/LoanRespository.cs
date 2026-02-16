using Microsoft.EntityFrameworkCore;
using pBiblioteca.Models;

public class LoanRepository : ILoanRepository
{
    private PostgresContext _context;

    public LoanRepository(PostgresContext context)
    {
        _context = context;
    }

    // =========================
    // LISTAR TODOS
    // =========================
    public List<TbLoan> SelectLoans()
    {
        return _context.TbLoans.ToList();
    }

    // =========================
    // BUSCAR POR ID
    // =========================
    public TbLoan? GetLoanById(int id)
    {
        return _context.TbLoans.FirstOrDefault(l => l.Id == id);
    }

    // =========================
    // CRIAR EMPRÉSTIMO
    // =========================
    public bool InsertLoan(string userCpf, string bookIsbn, DateOnly loanDate, DateOnly dueDate)
    {
        TbLoan loan = new TbLoan
        {
            UserCpf = userCpf,
            BookIsbn = bookIsbn,
            Loandate = loanDate,
            Duedate = dueDate,
            Returndate = null,
            Status = true
        };

        _context.TbLoans.Add(loan);
        _context.SaveChanges();

        return true;
    }

    // =========================
    // ATUALIZAR (DEVOLUÇÃO)
    // =========================
    public bool UpdateLoan(int id, DateOnly? returnDate, bool status)
    {
        TbLoan? loan = _context.TbLoans.FirstOrDefault(l => l.Id == id);

        if (loan == null)
            return false;

        loan.Returndate = returnDate;
        loan.Status = status;

        _context.SaveChanges();
        return true;
    }
}
