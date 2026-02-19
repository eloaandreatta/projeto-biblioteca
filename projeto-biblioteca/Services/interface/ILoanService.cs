using pBiblioteca.Models;

public interface ILoanService
{
    List<LoanResponseDTO> GetLoans(
        bool? status,
        string? userCpf,
        string? bookIsbn
    );
    public LoanResponseDTO? GetLoanById(int id);
    public List<LoanResponseDTO> GetLoansByUser(string cpf);
    public string CreateLoan(CreateLoanRequest request);
    string ReturnLoan(int id);
    string RenewLoan(int id);

}
