using pBiblioteca.Models;

public interface IFineRepository
{
    List<TbFine> SelectFinesByCpf(string userCpf);
    TbFine? GetFineById(int id);
    TbFine? GetFineByLoanId(int loanId);

    bool InsertFine(string userCpf, int loanId, decimal amount, int daysLate, decimal dailyRate);
    bool UpdateFine(int id, decimal amount, int daysLate, decimal dailyRate);
    bool PayFine(int id, DateOnly paymentDate);

    // ✅ Regra 2: checar multa em aberto
    bool HasOpenFineByCpf(string userCpf);
}
