using pBiblioteca.Models;

public interface IUserRepository
{
    public List<TbUser> SelectUsers();

    public List<TbUser> SelectUsersWithOrders();
    public void UpdateUser(string Cpf, string newPassword);
    public TbUser? GetUserById(string Cpf);
}