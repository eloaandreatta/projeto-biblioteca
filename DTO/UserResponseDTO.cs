using pBiblioteca.Models;

public class UserResponseDTO
{
    public string Cpf {get;set;} = null!;
    public string Email {get;set;} = null!;
    public string Nome {get;set;} = null!;
}

public class UpdateUserRequest
{
    public string Senha {get;set;} = null!;
}