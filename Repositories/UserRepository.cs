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

    public TbUser? GetUserById(Guid id){
        TbUser? findedUser = dbContext.TbUsers.Find(id);
        return findedUser;
    }

    public void UpdateUser(Guid id, string newPassword){
        TbUser? findedUser = dbContext.TbUsers.Find(id);
        findedUser.Password = newPassword;
        dbContext.SaveChanges();
    }
}