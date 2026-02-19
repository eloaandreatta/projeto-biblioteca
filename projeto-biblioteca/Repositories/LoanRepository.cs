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
        return _context.TbLoans
        .OrderByDescending(l => l.Id)
        .ToList();
    }

    // =========================
    // BUSCAR POR ID
    // =========================
    public TbLoan? GetLoanById(int id)
    {
        return _context.TbLoans.FirstOrDefault(l => l.Id == id);
    }

    // =========================
    // BUSCAR POR CPF DO USUÁRIO
    // =========================
    public List<TbLoan> GetLoansByUserCpf(string cpf)
    {
        return _context.TbLoans
            .Where(l => l.UserCpf == cpf)
            .OrderByDescending(l => l.Id)
            .ToList();
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
            Status = true
        };

        _context.TbLoans.Add(loan);

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

        return true;
    }

    public bool RenewLoan(int id, DateOnly newDueDate)
    {
        TbLoan? loan = _context.TbLoans.FirstOrDefault(l => l.Id == id);

        if (loan == null)
            return false;

        loan.Duedate = newDueDate;

        return true;
    }


    public TbUser? GetUserByCpf(string cpf)
    {
        return _context.TbUsers.FirstOrDefault(u => u.Cpf == cpf);
    }

    public TbBook? GetBookByIsbn(string isbn)
    {
        return _context.TbBooks.FirstOrDefault(b => b.Isbn == isbn);
    }

    public bool UserHasActiveLoan(string cpf)
    {
        return _context.TbLoans.Any(l => l.UserCpf == cpf && l.Status == true);
    }

    public bool UserHasUnpaidFine(string cpf)
    {
        return _context.TbFines.Any(f => f.UserCpf == cpf && !f.Ispaid);
    }

    public void AddFine(TbFine fine)
    {
        _context.TbFines.Add(fine);
    }

    public void Save()
    {
        _context.SaveChanges();
    }

}
