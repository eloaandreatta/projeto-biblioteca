using pBiblioteca.Models;

public interface ILoanService
{
    // Lista com filtros (serve também para "sem filtro" passando null, null, null)
    List<LoanResponseDTO> GetLoans(bool? status, string? userCpf, string? bookIsbn);

    LoanResponseDTO? GetLoanById(int id);
    List<LoanResponseDTO> GetLoansByUser(string cpf);

    string CreateLoan(CreateLoanRequest request);
    string ReturnLoan(int id);
    string RenewLoan(int id);

    // JOIN: Empréstimos + Usuário + Livro
    List<LoanDetailsDTO> GetLoanDetails(bool? status, string? userCpf, string? bookIsbn);
}