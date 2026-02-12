using pBiblioteca.Models;
using Microsoft.EntityFrameworkCore;

// Responsavel pela conexao com o banco de dados
public class UserRepository : IUserRepository
{
    private PostgresContext dbContext;

    public UserRepository(PostgresContext context)
    {
        dbContext = context;
    }

    public List<TbUser> SelectUsers()
    {
         List<TbUser> tbUsers = dbContext.TbUsers.ToList();
         return tbUsers;
    }

    public List<TbUser> SelectUsersWithOrders()
    {
        List<TbUser> tbUsers = dbContext.TbUsers.Include(u => u.TbReservations).ToList();
        return tbUsers;
    }

    public TbUser? GetUserById(string Cpf){
        TbUser? findedUser = dbContext.TbUsers.Find(Cpf);
        return findedUser;
    }

    public void UpdateUser(string Cpf, string newPassword){
        TbUser? findedUser = dbContext.TbUsers.Find(Cpf);
        findedUser.Password = newPassword;
        dbContext.SaveChanges();
    }
}