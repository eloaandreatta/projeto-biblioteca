using pBiblioteca.DTO;
using pBiblioteca.Models;

public interface IUserRepository
{
    List<TbUser> SelectUsers();
    TbUser? GetUserById(string cpf);
    void UpdateUserData(string cpf, UpdateUserRequestDTO request);
}

