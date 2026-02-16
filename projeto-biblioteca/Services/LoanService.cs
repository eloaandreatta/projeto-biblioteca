using pBiblioteca.Models;

public class LoanService : ILoanService
{
    private ILoanRepository _repository;

    public LoanService(ILoanRepository repository)
    {
        _repository = repository;
    }

    // GET ALL
    public List<LoanResponseDTO> GetLoans()
    {
        List<TbLoan> tbLoans = _repository.SelectLoans();
        List<LoanResponseDTO> loansDTO = new List<LoanResponseDTO>();

        foreach (TbLoan tbLoan in tbLoans)
        {
            LoanResponseDTO loanDTO = new LoanResponseDTO
            {
                Id = tbLoan.Id,
                UserCpf = tbLoan.UserCpf,
                BookIsbn = tbLoan.BookIsbn,
                LoanDate = tbLoan.Loandate,
                DueDate = tbLoan.Duedate,
                ReturnDate = tbLoan.Returndate,
                Status = tbLoan.Status
            };

            loansDTO.Add(loanDTO);
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

    // CREATE LOAN
    public string CreateLoan(CreateLoanRequest request)
    {
        if (request == null) return "error";
        if (string.IsNullOrWhiteSpace(request.UserCpf)) return "error";
        if (string.IsNullOrWhiteSpace(request.BookIsbn)) return "error";

        bool success = _repository.InsertLoan(
            request.UserCpf.Trim(),
            request.BookIsbn.Trim(),
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(request.DueDate)
        );

        if (!success)
            return "error";

        return "";
    }

    // RETURN LOAN
    public string ReturnLoan(int id)
    {
        if (id <= 0)
            return "error";

        TbLoan? loan = _repository.GetLoanById(id);

        if (loan == null)
            return "error";

        if (!loan.Status) // já devolvido
            return "error";

        bool success = _repository.UpdateLoan(
            loan.Id,
            DateOnly.FromDateTime(DateTime.Now),
            false
        );

        if (!success)
            return "error";

        return "";
    }
}
