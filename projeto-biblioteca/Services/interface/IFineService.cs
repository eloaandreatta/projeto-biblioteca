using pBiblioteca.DTO;

public interface IFineService
{
    List<FineResponseDTO> GetFinesByCpf(string userCpf);
    FineResponseDTO? GetFineById(int id);

    // cria/atualiza multa se necessário
    FineResponseDTO? GetOrCreateFineByLoanId(int loanId);

    string PayFine(int fineId);

    // checar multa em aberto
    bool HasOpenFineByCpf(string userCpf);
}
