using pBiblioteca.Models;

public class UserResponseDTO
{
    public string Cpf {get;set;} = string.Empty;
    public string Email {get;set;} = string.Empty;
    public string Nome {get;set;} = string.Empty;
    public string Telefone {get;set;} = string.Empty;
    public string Endereco {get;set;} = string.Empty;
}

public class UpdateUserRequest
{
    public string Senha {get;set;} = string.Empty;
}