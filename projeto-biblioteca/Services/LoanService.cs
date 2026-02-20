using pBiblioteca.Models;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _repository;
    // REGRA DE NEGÓCIO:
    // Valor fixo da multa por dia de atraso.
    private const decimal DAILY_FINE_RATE = 0.50m;
    private const int LOAN_PERIOD_DAYS = 14;

    public LoanService(ILoanRepository repository)
    {
        _repository = repository;
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

    // GET BY ID
    public LoanResponseDTO? GetLoanById(int id)
    {
        if (id <= 0)
            return null;

        TbLoan? tbLoan = _repository.GetLoanById(id);

        if (tbLoan == null)
            return null;

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

    // GET BY CPF
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


    // CREATE LOAN
    public string CreateLoan(CreateLoanRequest request)
    {
        if (request == null) return "error";

        // REGRA:
        // Usuário precisa existir e estar ativo.
        var user = _repository.GetUserByCpf(request.UserCpf);
        if (user == null || !user.Active)
            return "error";

        // REGRA:
        // Usuário não pode emprestar mais de 1 livro simultaneamente.
        if (_repository.UserHasActiveLoan(request.UserCpf))
            return "error";

        // REGRA:
        // Usuário com multa não paga não pode realizar novos empréstimos.
        if (_repository.UserHasUnpaidFine(request.UserCpf))
            return "error";

        // REGRA:
        // Não permitir empréstimo se o livro não estiver disponível.
        var book = _repository.GetBookByIsbn(request.BookIsbn);
        if (book == null || book.Availablequantity <= 0)
            return "error"; //Aqui deveria criar a reserva automaticamente

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        // REGRA:
        // Prazo fixo de empréstimo: 14 dias após a data do empréstimo.
        DateOnly dueDate = today.AddDays(LOAN_PERIOD_DAYS);

        bool success = _repository.InsertLoan(
            request.UserCpf,
            request.BookIsbn,
            today,
            dueDate
        );

        if (!success)
            return "error";

        // Atualiza estoque (reduz disponibilidade)
        book.Availablequantity--;

        _repository.Save();

        return "";
    }


    // RETURN LOAN
    public string ReturnLoan(int id)
    {
        if (id <= 0)
            return "error";

        TbLoan? loan = _repository.GetLoanById(id);
        if (loan == null || !loan.Status)
            return "error";

        var book = _repository.GetBookByIsbn(loan.BookIsbn);
        if (book == null)
            return "error";

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        // verificar atraso
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

        book.Availablequantity++;

        bool success = _repository.UpdateLoan(
            loan.Id,
            today,
            false
        );

        _repository.Save();

        if (!success)
            return "error";

        return "";
    }

    public string RenewLoan(int id)
    {
        var loan = _repository.GetLoanById(id);

        if (loan == null)
            return "error";

        if (!loan.Status)
            return "error";

        if (_repository.UserHasUnpaidFine(loan.UserCpf))
            return "error";

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        // Regra de negócio: Não pode estar atrasado
        if (loan.Duedate < today)
            return "error";

        // Regra de negócio: Só pode renovar uma vez (máximo 28 dias totais)
        DateOnly maxDueDate = loan.Loandate.AddDays(28);

        if (loan.Duedate >= maxDueDate)
            return "error";

        DateOnly newDueDate = loan.Duedate.AddDays(14);

        bool updated = _repository.RenewLoan(id, newDueDate);

        if (!updated)
            return "error";

        _repository.Save();

        return "ok";
    }

    public List<LoanResponseDTO> GetLoans(bool? status, string? userCpf, string? bookIsbn)
    {
        var tbLoans = _repository.SelectLoans();

        if (status != null)
            tbLoans = tbLoans.Where(l => l.Status == status.Value).ToList();

        if (!string.IsNullOrWhiteSpace(userCpf))
            tbLoans = tbLoans.Where(l => l.UserCpf == userCpf).ToList();

        if (!string.IsNullOrWhiteSpace(bookIsbn))
            tbLoans = tbLoans.Where(l => l.BookIsbn == bookIsbn).ToList();

        return tbLoans.Select(l => new LoanResponseDTO
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

    public List<LoanDetailsDTO> GetLoanDetails(bool? status, string? userCpf, string? bookIsbn)
    {
        return _repository.SelectLoanDetails(status, userCpf, bookIsbn);
    }
}
