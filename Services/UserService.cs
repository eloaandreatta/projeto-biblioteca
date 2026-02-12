using pBiblioteca.Models;

// responsavel por toda logica do EndPoint
// Conecta ao repositorio
public class UserService : IUserService
{
    private IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public List<UserResponseDTO> GetUsers()
    {
        List<TbUser> tbUsers = _repository.SelectUsers();

        // crio uma lista com o tipo que quero do retorno
        List<UserResponseDTO> usersDTO = new List<UserResponseDTO>();

        // percorre a tabela de usuarios
        foreach(TbUser tbUser in tbUsers)
        {
            // cria um usuario com tipo do meu retorno
            UserResponseDTO usuarioRetorno = new UserResponseDTO();
            usuarioRetorno.Cpf = tbUser.Cpf;
            usuarioRetorno.Nome = tbUser.Name;
            usuarioRetorno.Email = tbUser.Email;

            // adiciona o usuario a minha lista de retorno
            usersDTO.Add(usuarioRetorno);
            // UserResponseDTO userDTO = new UserResponseDTO()
            // {
            //     Id = tbUser.Id,
            //     Nome = tbUser.Name,
            //     Email = tbUser.Email
            // };
            // usersDTO.Add(userDTO);
        }

        // retorna a lista que eu criei
        return usersDTO;
    }

    public string UpdateUserPassword(string Cpf, string password){
        TbUser? user = _repository.GetUserById(Cpf);
        if(user == null){
            return "error";
        }

        _repository.UpdateUser(Cpf, password);
        return "";
    }
}