public interface IUserService
{
    public List<UserResponseDTO> GetUsers();
     public string UpdateUserPassword(Guid id, string password);
}