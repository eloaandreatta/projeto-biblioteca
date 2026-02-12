using pBiblioteca.Models;
using pBiblioteca.DTO;
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
        return dbContext.TbUsers.ToList();
    }

    public TbUser? GetUserById(string cpf)
    {
        return dbContext.TbUsers.Find(cpf);
    }

    public TbUser? GetUserByTelephone(string telephone)
    {
        return dbContext.TbUsers
            .FirstOrDefault(u => u.Telephone == telephone);
    }

    public void AddUser(TbUser user)
    {
        dbContext.TbUsers.Add(user);
        dbContext.SaveChanges();
    }

    public void UpdateUserData(string cpf, UpdateUserRequestDTO request)
    {
        TbUser? user = dbContext.TbUsers.Find(cpf);

        if (user == null)
            return;

        user.Name = request.Nome;
        user.Email = request.Email;
        user.Telephone = request.Telefone;
        user.Address = request.Endereco;

        dbContext.SaveChanges();
    }
}
