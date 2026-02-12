public interface IUserService
{
    public List<UserResponseDTO> GetUsers();
     public string UpdateUserPassword(string Cpf, string password);
}