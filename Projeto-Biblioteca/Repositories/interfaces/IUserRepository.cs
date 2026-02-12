using pBiblioteca.DTO;
using pBiblioteca.Models;

    public interface IUserRepository
{
    List<TbUser> SelectUsers();

    TbUser? GetUserById(string cpf);

    TbUser? GetUserByTelephone(string telephone);

    void AddUser(TbUser user);

    void UpdateUserData(string cpf, UpdateUserRequestDTO request);
}



