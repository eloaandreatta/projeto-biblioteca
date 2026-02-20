using NUnit.Framework;
using Moq;
using pBiblioteca.Services;
using pBiblioteca.Repositories;
using pBiblioteca.Models;
using pBiblioteca.DTO;
using System.Collections.Generic;

namespace Projeto_Biblioteca.Tests
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _mockRepository;
        private UserService _service;

        // Executa antes de cada teste
        [SetUp]
        public void Setup()
        {
            // Cria um repositório falso (mock)
            _mockRepository = new Mock<IUserRepository>();

            // Injeta o mock no service real
            _service = new UserService(_mockRepository.Object);
        }

        // ===============================
        // GET USERS
        // ===============================

        [Test]
        public void GetUsers_DeveRetornarListaMapeada()
        {
            // Arrange - Simula usuários vindos do banco
            var tbUsers = new List<TbUser>
            {
                new TbUser { Cpf="1", Name="A", Email="a@email.com", Telephone="111", Address="Rua 1" },
                new TbUser { Cpf="2", Name="B", Email="b@email.com", Telephone="222", Address="Rua 2" }
            };

            // Define comportamento do mock
            _mockRepository.Setup(r => r.SelectUsers()).Returns(tbUsers);

            // Act - Executa método real
            var result = _service.GetUsers();

            // Assert - Verifica se o mapeamento foi feito corretamente
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Cpf, Is.EqualTo("1"));
            Assert.That(result[0].Name, Is.EqualTo("A"));
        }

        [Test]
        public void GetUsers_DeveRetornarListaVazia()
        {
            // Arrange - Simula banco vazio
            _mockRepository.Setup(r => r.SelectUsers())
                           .Returns(new List<TbUser>());

            // Act
            var result = _service.GetUsers();

            // Assert - Deve retornar lista vazia
            Assert.That(result.Count, Is.EqualTo(0));
        }

        // ===============================
        // CREATE USER
        // ===============================

        [Test]
        public void CreateUser_DeveRetornarErro_QuandoRequestNulo()
        {
            // Act
            var result = _service.CreateUser(null!);

            // Assert
            Assert.That(result, Is.EqualTo("error"));
            _mockRepository.Verify(r => r.AddUser(It.IsAny<TbUser>()), Times.Never);
        }

        [Test]
        public void CreateUser_DeveRetornarErro_QuandoCpfVazio()
        {
            // Arrange - CPF inválido
            var request = new CreateUserRequestDTO
            {
                Cpf = "",
                Name = "Nome",
                Email = "email@email.com",
                Telephone = "111",
                Address = "Rua",
                Password = "123"
            };

            // Act
            var result = _service.CreateUser(request);

            // Assert - Deve barrar antes de acessar o repositório
            Assert.That(result, Is.EqualTo("CPF é obrigatório."));
            _mockRepository.Verify(r => r.AddUser(It.IsAny<TbUser>()), Times.Never);
        }

        [Test]
        public void CreateUser_DeveRetornarErro_QuandoNomeVazio()
        {
            // Arrange - Nome inválido
            var request = new CreateUserRequestDTO
            {
                Cpf = "123",
                Name = "",
                Email = "email@email.com",
                Telephone = "111",
                Address = "Rua",
                Password = "123"
            };

            // Act
            var result = _service.CreateUser(request);

            // Assert
            Assert.That(result, Is.EqualTo("Nome é obrigatório."));
            _mockRepository.Verify(r => r.AddUser(It.IsAny<TbUser>()), Times.Never);
        }

        [Test]
        public void CreateUser_DeveRetornarErro_QuandoEmailInvalido()
        {
            // Arrange - Email sem "@"
            var request = new CreateUserRequestDTO
            {
                Cpf = "123",
                Name = "Nome",
                Email = "emailinvalido",
                Telephone = "111",
                Address = "Rua",
                Password = "123"
            };

            // Act
            var result = _service.CreateUser(request);

            // Assert
            Assert.That(result, Is.EqualTo("Email inválido."));
            _mockRepository.Verify(r => r.AddUser(It.IsAny<TbUser>()), Times.Never);
        }

        [Test]
        public void CreateUser_DeveRetornarErro_QuandoUsuarioJaExiste()
        {
            // Arrange - Simula CPF já cadastrado
            var request = new CreateUserRequestDTO
            {
                Cpf = "123",
                Name = "Nome",
                Email = "email@email.com",
                Telephone = "111",
                Address = "Rua",
                Password = "123"
            };

            _mockRepository.Setup(r => r.GetUserById("123"))
                           .Returns(new TbUser());

            // Act
            var result = _service.CreateUser(request);

            // Assert - Não deve permitir duplicidade
            Assert.That(result, Is.EqualTo("Usuário já cadastrado."));
            _mockRepository.Verify(r => r.AddUser(It.IsAny<TbUser>()), Times.Never);
        }

        [Test]
        public void CreateUser_DeveRetornarErro_QuandoTelefoneJaExiste()
        {
            // Arrange - CPF não existe, mas telefone já existe
            var request = new CreateUserRequestDTO
            {
                Cpf = "123",
                Name = "Nome",
                Email = "email@email.com",
                Telephone = "111",
                Address = "Rua",
                Password = "123"
            };

            _mockRepository.Setup(r => r.GetUserById("123"))
                           .Returns((TbUser?)null);

            _mockRepository.Setup(r => r.GetUserByTelephone("111"))
                           .Returns(new TbUser());

            // Act
            var result = _service.CreateUser(request);

            // Assert
            Assert.That(result, Is.EqualTo("Telefone já cadastrado"));
            _mockRepository.Verify(r => r.AddUser(It.IsAny<TbUser>()), Times.Never);
        }

        [Test]
        public void CreateUser_DeveCadastrarERetornarSucesso()
        {
            // Arrange - Usuário válido e inexistente
            var request = new CreateUserRequestDTO
            {
                Cpf = "123",
                Name = "Nome",
                Email = "email@email.com",
                Telephone = "111",
                Address = "Rua",
                Password = "123"
            };

            _mockRepository.Setup(r => r.GetUserById("123"))
                           .Returns((TbUser?)null);

            _mockRepository.Setup(r => r.GetUserByTelephone("111"))
                           .Returns((TbUser?)null);

            // Act
            var result = _service.CreateUser(request);

            // Assert - Deve salvar no repositório
            Assert.That(result, Is.EqualTo("Usuário cadastrado com sucesso."));
            _mockRepository.Verify(r => r.AddUser(It.IsAny<TbUser>()), Times.Once);
        }

        // ===============================
        // UPDATE USER
        // ===============================

        [Test]
        public void UpdateUser_DeveRetornarOk()
        {
            string cpf = "123";

            // Simula usuário existente
            _mockRepository.Setup(r => r.GetUserById(cpf))
                           .Returns(new TbUser());

            // Act
            var result = _service.UpdateUser(cpf, new UpdateUserRequestDTO());

            // Assert
            Assert.That(result, Is.EqualTo("ok"));
            _mockRepository.Verify(r => r.UpdateUserData(cpf, It.IsAny<UpdateUserRequestDTO>()), Times.Once);
        }

        [Test]
        public void UpdateUser_DeveRetornarError_QuandoNaoExistir()
        {
            string cpf = "123";

            // Simula usuário inexistente
            _mockRepository.Setup(r => r.GetUserById(cpf))
                           .Returns((TbUser?)null);

            // Act
            var result = _service.UpdateUser(cpf, new UpdateUserRequestDTO());

            // Assert
            Assert.That(result, Is.EqualTo("error"));
            _mockRepository.Verify(r => r.UpdateUserData(It.IsAny<string>(), It.IsAny<UpdateUserRequestDTO>()), Times.Never);
        }

        // ===============================
        // DELETE USER
        // ===============================

        [Test]
        public void DeleteUser_DeveRetornarNaoEncontrado()
        {
            string cpf = "123";

            // Simula usuário inexistente
            _mockRepository.Setup(r => r.GetUserById(cpf))
                           .Returns((TbUser?)null);

            // Act
            var result = _service.DeleteUser(cpf);

            // Assert
            Assert.That(result, Is.EqualTo("não encontrado"));
            _mockRepository.Verify(r => r.DeleteUser(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void DeleteUser_DeveRetornarOk()
        {
            string cpf = "123";

            // Simula usuário existente
            _mockRepository.Setup(r => r.GetUserById(cpf))
                           .Returns(new TbUser());

            // Act
            var result = _service.DeleteUser(cpf);

            // Assert
            Assert.That(result, Is.EqualTo("ok"));
            _mockRepository.Verify(r => r.DeleteUser(cpf), Times.Once);
        }
    }
}
