using pBiblioteca.DTO;

public interface IUserService
{
    List<UserResponseDTO> GetUsers();

    string CreateUser(CreateUserRequestDTO request);

    string UpdateUser(string cpf, UpdateUserRequestDTO request);
}
