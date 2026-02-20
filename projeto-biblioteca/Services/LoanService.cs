using pBiblioteca.Models;
using pBiblioteca.Services;
namespace pBiblioteca.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _repository;
    private readonly IFineRepository _fineRepo;
    private readonly IReservationService _reservationService; // ✅ NOVO

    private const decimal DAILY_FINE_RATE = 0.50m;
    private const int LOAN_PERIOD_DAYS = 14;
    private const int RESERVATION_PICKUP_DAYS = 3; // ✅ NOVO (pra notificar)

    public LoanService(
        ILoanRepository repository,
        IFineRepository fineRepo,
        IReservationService reservationService) // ✅ NOVO
    {
        _repository = repository;
        _fineRepo = fineRepo;
        _reservationService = reservationService;
    }

    public List<LoanResponseDTO> GetLoans()
    {
        List<TbLoan> tbLoans = _repository.SelectLoans();
        List<LoanResponseDTO> loansDTO = new List<LoanResponseDTO>();

        foreach (TbLoan tbLoan in tbLoans)
        {
            loansDTO.Add(new LoanResponseDTO
            {
                Id = tbLoan.Id,
                UserCpf = tbLoan.UserCpf,
                BookIsbn = tbLoan.BookIsbn,
                LoanDate = tbLoan.Loandate,
                DueDate = tbLoan.Duedate,
                ReturnDate = tbLoan.Returndate,
                Status = tbLoan.Status
            });
        }

        return loansDTO;
    }

    public LoanResponseDTO? GetLoanById(int id)
    {
        if (id <= 0) return null;

        TbLoan? tbLoan = _repository.GetLoanById(id);
        if (tbLoan == null) return null;

        return new LoanResponseDTO
        {
            Id = tbLoan.Id,
            UserCpf = tbLoan.UserCpf,
            BookIsbn = tbLoan.BookIsbn,
            LoanDate = tbLoan.Loandate,
            DueDate = tbLoan.Duedate,
            ReturnDate = tbLoan.Returndate,
            Status = tbLoan.Status
        };
    }

    public List<LoanResponseDTO> GetLoansByUser(string cpf)
    {
        var loans = _repository.GetLoansByUserCpf(cpf);

        return loans.Select(l => new LoanResponseDTO
        {
            Id = l.Id,
            UserCpf = l.UserCpf,
            BookIsbn = l.BookIsbn,
            LoanDate = l.Loandate,
            DueDate = l.Duedate,
            ReturnDate = l.Returndate,
            Status = l.Status
        }).ToList();
    }

    // ✅ CREATE LOAN (Regra 2: auto-reserva quando indisponível)
    public string CreateLoan(CreateLoanRequest request)
    {
        if (request == null) return "error";

        var user = _repository.GetUserByCpf(request.UserCpf);
        if (user == null || !user.Active) return "error";

        if (_repository.UserHasActiveLoan(request.UserCpf))
            return "error";

        // ✅ Regra 1: multa em aberto bloqueia
        if (_fineRepo.HasOpenFineByCpf(request.UserCpf))
            return "error";

        var book = _repository.GetBookByIsbn(request.BookIsbn);
        if (book == null)
            return "error";

        // ✅ Regra 2: se não tem disponível, entra automaticamente na fila de reserva
        if (book.Availablequantity <= 0)
        {
            var reservationResult = _reservationService.CreateReservation(new CreateReservationRequest
            {
                UserCpf = request.UserCpf,
                BookIsbn = request.BookIsbn
            });

                if (reservationResult == "user_has_unpaid_fine")
                    return "user_has_unpaid_fine";

                if (reservationResult == "user_not_found")
                    return "error";

                if (reservationResult == "book_not_found")
                    return "error";

                if (reservationResult == "reservation_already_exists")
                    return "queued_for_reservation";

                if (reservationResult == "ok")
                    return "queued_for_reservation";

            return "error";
        }


        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        DateOnly dueDate = today.AddDays(LOAN_PERIOD_DAYS);

        bool success = _repository.InsertLoan(
            request.UserCpf,
            request.BookIsbn,
            today,
            dueDate
        );

        if (!success) return "error";

        book.Availablequantity--;
        _repository.Save();

        return "";
    }

    // ✅ RETURN LOAN (Regra 4: notificar primeiro da fila + prazo 3 dias)
    public string ReturnLoan(int id)
    {
        if (id <= 0) return "error";

        TbLoan? loan = _repository.GetLoanById(id);
        if (loan == null || !loan.Status) return "error";

        var book = _repository.GetBookByIsbn(loan.BookIsbn);
        if (book == null) return "error";

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        // Regra: multa automática se atrasar
        if (today > loan.Duedate)
        {
            int daysLate = today.DayNumber - loan.Duedate.DayNumber;
            decimal amount = daysLate * DAILY_FINE_RATE;

            TbFine fine = new TbFine
            {
                UserCpf = loan.UserCpf,
                LoanId = loan.Id,
                Amount = amount,
                Dayslate = daysLate,
                Ispaid = false,
                Dailyrate = DAILY_FINE_RATE,
                Paymentdate = null
            };

            _repository.AddFine(fine);
        }

        // devolve o livro (estoque)
        book.Availablequantity++;

        // atualiza o empréstimo (fecha)
        bool success = _repository.UpdateLoan(
            loan.Id,
            today,
            false
        );

        if (!success) return "error";

        // Só notifica se realmente houver estoque disponível
        if (book.Availablequantity > 0)
        {
            _reservationService.NotifyNextIfAny(
                loan.BookIsbn,
                today,
                today.AddDays(RESERVATION_PICKUP_DAYS)
            );
        }

        // ✅ Salva tudo do contexto do LoanRepository (multa + devolução + estoque)
        _repository.Save();

        return "";
    }


    public string RenewLoan(int id)
    {
        var loan = _repository.GetLoanById(id);
        if (loan == null) return "error";
        if (!loan.Status) return "error";

        if (_fineRepo.HasOpenFineByCpf(loan.UserCpf))
            return "error";

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        if (loan.Duedate < today)
            return "error";

        DateOnly maxDueDate = loan.Loandate.AddDays(28);
        if (loan.Duedate >= maxDueDate)
            return "error";

        DateOnly newDueDate = loan.Duedate.AddDays(14);

        bool updated = _repository.RenewLoan(id, newDueDate);
        if (!updated) return "error";

        _repository.Save();
        return "ok";
    }

    public List<LoanResponseDTO> GetLoans(bool? status, string? userCpf, string? bookIsbn)
    {
        var tbLoans = _repository.SelectLoans();

        if (status.HasValue)
            tbLoans = tbLoans.Where(l => l.Status == status.Value).ToList();

        if (!string.IsNullOrEmpty(userCpf))
            tbLoans = tbLoans.Where(l => l.UserCpf == userCpf).ToList();

        if (!string.IsNullOrEmpty(bookIsbn))
            tbLoans = tbLoans.Where(l => l.BookIsbn == bookIsbn).ToList();

        return tbLoans.Select(tbLoan => new LoanResponseDTO
        {
            Id = tbLoan.Id,
            UserCpf = tbLoan.UserCpf,
            BookIsbn = tbLoan.BookIsbn,
            LoanDate = tbLoan.Loandate,
            DueDate = tbLoan.Duedate,
            ReturnDate = tbLoan.Returndate,
            Status = tbLoan.Status
        }).ToList();
    }

}
