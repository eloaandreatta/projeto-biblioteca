using pBiblioteca.DTO;
using pBiblioteca.Models;
namespace pBiblioteca.Services;
public class FineService : IFineService
{
    private readonly IFineRepository _fineRepository;
    private readonly ILoanRepository _loanRepository;

    private const decimal DAILY_RATE = 0.30m;

    public FineService(IFineRepository fineRepository, ILoanRepository loanRepository)
    {
        _fineRepository = fineRepository;
        _loanRepository = loanRepository;
    }

    public List<FineResponseDTO> GetFinesByCpf(string userCpf)
    {
        if (string.IsNullOrWhiteSpace(userCpf))
            return new List<FineResponseDTO>();

        var fines = _fineRepository.SelectFinesByCpf(userCpf.Trim());

        return fines.Select(f => new FineResponseDTO
        {
            Id = f.Id,
            UserCpf = f.UserCpf,
            LoanId = f.LoanId,
            Amount = f.Amount,
            DaysLate = f.Dayslate,
            IsPaid = f.Ispaid,
            DailyRate = f.Dailyrate,
            PaymentDate = f.Paymentdate
        }).ToList();
    }

    public FineResponseDTO? GetFineById(int id)
    {
        if (id <= 0) return null;

        TbFine? f = _fineRepository.GetFineById(id);
        if (f == null) return null;

        return new FineResponseDTO
        {
            Id = f.Id,
            UserCpf = f.UserCpf,
            LoanId = f.LoanId,
            Amount = f.Amount,
            DaysLate = f.Dayslate,
            IsPaid = f.Ispaid,
            DailyRate = f.Dailyrate,
            PaymentDate = f.Paymentdate
        };
    }

    public FineResponseDTO? GetOrCreateFineByLoanId(int loanId)
    {
        if (loanId <= 0) return null;

        TbLoan? loan = _loanRepository.GetLoanById(loanId);
        if (loan == null) return null;

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        // devolvido? usa returndate, senão hoje
        DateOnly endDate = loan.Returndate ?? today;

        int daysLate = endDate.DayNumber - loan.Duedate.DayNumber;
        if (daysLate <= 0) return null; // ✅ Regra 1: sem atraso = sem multa

        decimal amount = Math.Round(daysLate * DAILY_RATE, 2);

        TbFine? fine = _fineRepository.GetFineByLoanId(loanId);

        if (fine == null)
        {
            _fineRepository.InsertFine(
                loan.UserCpf,
                loan.Id,
                amount,
                daysLate,
                DAILY_RATE
            );

            fine = _fineRepository.GetFineByLoanId(loanId);
        }
        else
        {
            // recalcula enquanto não pagou
            if (!fine.Ispaid)
            {
                _fineRepository.UpdateFine(fine.Id, amount, daysLate, DAILY_RATE);
                fine = _fineRepository.GetFineByLoanId(loanId);
            }
        }

        if (fine == null) return null;

        return new FineResponseDTO
        {
            Id = fine.Id,
            UserCpf = fine.UserCpf,
            LoanId = fine.LoanId,
            Amount = fine.Amount,
            DaysLate = fine.Dayslate,
            IsPaid = fine.Ispaid,
            DailyRate = fine.Dailyrate,
            PaymentDate = fine.Paymentdate
        };
    }

    public string PayFine(int fineId)
    {
        if (fineId <= 0) return "error";

        TbFine? fine = _fineRepository.GetFineById(fineId);
        if (fine == null) return "error";
        if (fine.Ispaid) return "error";

        bool ok = _fineRepository.PayFine(fineId, DateOnly.FromDateTime(DateTime.Now));
        return ok ? "" : "error";
    }

    public bool HasOpenFineByCpf(string userCpf)
    {
        if (string.IsNullOrWhiteSpace(userCpf)) return false;
        return _fineRepository.HasOpenFineByCpf(userCpf.Trim());
    }
}
