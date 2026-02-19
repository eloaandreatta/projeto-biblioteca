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
                usuarioRetorno.Name = tbUser.Name;
                usuarioRetorno.Email = tbUser.Email;
                usuarioRetorno.Telephone = tbUser.Telephone;
                usuarioRetorno.Address = tbUser.Address;

                usersDTO.Add(usuarioRetorno);
            }

            return usersDTO;
        }

        // CRIAR CADASTRO
        public string CreateUser(CreateUserRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Cpf))
                return "CPF é obrigatório.";

            if (string.IsNullOrWhiteSpace(request.Name))
                return "Nome é obrigatório.";

            if (!request.Email.Contains("@"))
                return "Email inválido.";

            var existingUser = _repository.GetUserById(request.Cpf);

            if (existingUser != null)
                return "Usuário já cadastrado.";

            var existingTelephone = _repository.GetUserByTelephone(request.Telephone);

            if (existingTelephone != null)
                return "Telefone já cadastrado.";

            TbUser user = new TbUser
            {
                Cpf = request.Cpf,
                Name = request.Name,
                Email = request.Email,
                Telephone = request.Telephone,
                Address = request.Address,
                Password = request.Password,
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
        
        // DELETAR CADASTRO
        public string DeleteUser(string cpf)
        {
            TbUser? user = _repository.GetUserById(cpf);

            if (user == null)
                return "não encontrado";

            _repository.DeleteUser(cpf);

            return "ok";
        }
       
        }

}