
using pBiblioteca.DTO;

public interface IUserService
{
    List<UserResponseDTO> GetUsers();
    string UpdateUser(string cpf, UpdateUserRequestDTO request);
}