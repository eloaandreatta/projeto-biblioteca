using pBiblioteca.Models;

public class FineRepository : IFineRepository
{
    private PostgresContext _context;

    public FineRepository(PostgresContext context)
    {
        _context = context;
    }

    public List<TbFine> SelectFinesByCpf(string userCpf)
    {
        return _context.TbFines
            .Where(f => f.UserCpf == userCpf)
            .ToList();
    }

    public TbFine? GetFineById(int id)
    {
        return _context.TbFines.FirstOrDefault(f => f.Id == id);
    }

    public TbFine? GetFineByLoanId(int loanId)
    {
        return _context.TbFines.FirstOrDefault(f => f.LoanId == loanId);
    }

    public bool InsertFine(string userCpf, int loanId, decimal amount, int daysLate, decimal dailyRate)
    {
        TbFine fine = new TbFine
        {
            UserCpf = userCpf,
            LoanId = loanId,
            Amount = amount,
            Dayslate = daysLate,
            Dailyrate = dailyRate,
            Ispaid = false,

            // Como Paymentdate não é nulo no seu model:
            Paymentdate = DateOnly.FromDateTime(DateTime.MinValue)
        };

        _context.TbFines.Add(fine);
        _context.SaveChanges();
        return true;
    }

    public bool UpdateFine(int id, decimal amount, int daysLate, decimal dailyRate)
    {
        TbFine? fine = _context.TbFines.FirstOrDefault(f => f.Id == id);
        if (fine == null) return false;
        if (fine.Ispaid) return false;

        fine.Amount = amount;
        fine.Dayslate = daysLate;
        fine.Dailyrate = dailyRate;

        _context.SaveChanges();
        return true;
    }

    public bool PayFine(int id, DateOnly paymentDate)
    {
        TbFine? fine = _context.TbFines.FirstOrDefault(f => f.Id == id);
        if (fine == null) return false;
        if (fine.Ispaid) return false;

        fine.Ispaid = true;
        fine.Paymentdate = paymentDate;

        _context.SaveChanges();
        return true;
    }

    public bool HasOpenFineByCpf(string userCpf)
    {
        return _context.TbFines.Any(f => f.UserCpf == userCpf && f.Ispaid == false);
    }
}
