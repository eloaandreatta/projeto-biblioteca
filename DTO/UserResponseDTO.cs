using pBiblioteca.Models;

public class UserResponseDTO
{
    public string Cpf {get;set;}
    public string Email {get;set;}
    public string Nome {get;set;}
    public string Telefone {get;set;} 
    public string Endereco {get;set;} 
}

public class UpdateUserRequest
{
    public string Senha {get;set;}
}