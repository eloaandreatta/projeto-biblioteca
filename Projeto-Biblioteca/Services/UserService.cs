using pBiblioteca.Models;
using pBiblioteca.DTO;
using pBiblioteca.Repositories;

// responsavel por toda logica do EndPoint
// Conecta ao repositorio
namespace pBiblioteca.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }


        // CONSULTAR CADASTRO
        public List<UserResponseDTO> GetUsers()
        {
            List<TbUser> tbUsers = _repository.SelectUsers();

            List<UserResponseDTO> usersDTO = new List<UserResponseDTO>();

            foreach (TbUser tbUser in tbUsers)
            {
                UserResponseDTO usuarioRetorno = new UserResponseDTO();

                usuarioRetorno.Cpf = tbUser.Cpf;
                usuarioRetorno.Nome = tbUser.Name;
                usuarioRetorno.Email = tbUser.Email;
                usuarioRetorno.Telefone = tbUser.Telephone;
                usuarioRetorno.Endereco = tbUser.Address;

                usersDTO.Add(usuarioRetorno);
            }

            return usersDTO;
        }

        // CRIAR CADASTRO
        public string CreateUser(CreateUserRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Cpf))
                return "CPF é obrigatório.";

            if (string.IsNullOrWhiteSpace(request.Nome))
                return "Nome é obrigatório.";

            if (!request.Email.Contains("@"))
                return "Email inválido.";

            var existingUser = _repository.GetUserById(request.Cpf);

            if (existingUser != null)
                return "Usuário já cadastrado.";

            var existingTelephone = _repository.GetUserByTelephone(request.Telefone);

            if (existingTelephone != null)
                return "Telefone já cadastrado.";

            TbUser user = new TbUser
            {
                Cpf = request.Cpf,
                Name = request.Nome,
                Email = request.Email,
                Telephone = request.Telefone,
                Address = request.Endereco,
                Password = request.Senha,
                Active = true
            };

            _repository.AddUser(user);

            return "Usuário cadastrado com sucesso.";
        }


        // ATUALIZAR CADASTRO
        public string UpdateUser(string cpf, UpdateUserRequestDTO request)
        {
            TbUser? user = _repository.GetUserById(cpf);

            if (user == null)
                return "error";

            _repository.UpdateUserData(cpf, request);

            return "ok";
        }
    }
}

