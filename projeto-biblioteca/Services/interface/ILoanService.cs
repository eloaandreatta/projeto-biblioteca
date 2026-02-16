using pBiblioteca.Models;

public interface ILoanService
{
    List<LoanResponseDTO> GetLoans();
    LoanResponseDTO? GetLoanById(int id);
    string CreateLoan(CreateLoanRequest request);
    string ReturnLoan(int id);
}
