using pBiblioteca.Models;

public class UserResponseDTO
{
    public string Cpf {get;set;} = string.Empty;
    public string Name {get;set;} = string.Empty;
    public string Telephone {get;set;} = string.Empty;
    public string Email {get;set;} = string.Empty;
    public string Address {get;set;} = string.Empty;
}

public class UpdateUserRequest
{
    public string Password {get;set;} = string.Empty;
}

public class UpdateUserRequestDTO
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class CreateUserRequestDTO
{
    public string Cpf { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;  
}