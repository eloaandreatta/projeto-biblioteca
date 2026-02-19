using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using pBiblioteca.Controllers;
using pBiblioteca.DTO;
using pBiblioteca.Services;
using System.Collections.Generic;

namespace Projeto_Biblioteca.Tests
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserService> _mockService;
        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IUserService>();
            _controller = new UserController(_mockService.Object);
        }

        // ===============================
        // GET
        // ===============================

        [Test]
        public void Get_DeveRetornarListaDeUsuarios()
        {
            // Arrange
            var users = new List<UserResponseDTO>
            {
                new UserResponseDTO { Cpf = "1", Name = "A" },
                new UserResponseDTO { Cpf = "2", Name = "B" }
            };

            _mockService.Setup(s => s.GetUsers()).Returns(users);

            // Act
            var result = _controller.Get();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Cpf, Is.EqualTo("1"));
            _mockService.Verify(s => s.GetUsers(), Times.Once);
        }

        [Test]
        public void Get_DeveRetornarListaVazia()
        {
            // Arrange
            _mockService.Setup(s => s.GetUsers()).Returns(new List<UserResponseDTO>());

            // Act
            var result = _controller.Get();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
            _mockService.Verify(s => s.GetUsers(), Times.Once);
        }

        // ===============================
        // POST - CreateUser
        // ===============================

        [Test]
        public void CreateUser_DeveRetornarOk_QuandoSucesso()
        {
            // Arrange
            var request = new CreateUserRequestDTO
            {
                Cpf = "123",
                Name = "Nome",
                Email = "email@email.com",
                Telephone = "111",
                Address = "Rua",
                Password = "123"
            };

            _mockService.Setup(s => s.CreateUser(It.IsAny<CreateUserRequestDTO>()))
                        .Returns("Usuário cadastrado com sucesso.");

            // Act
            var actionResult = _controller.CreateUser(request);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var ok = (OkObjectResult)actionResult;
            Assert.That(ok.Value, Is.EqualTo("Usuário cadastrado com sucesso."));
            _mockService.Verify(s => s.CreateUser(It.IsAny<CreateUserRequestDTO>()), Times.Once);
        }

        [Test]
        public void CreateUser_DeveRetornarBadRequest_QuandoErro()
        {
            // Arrange
            var request = new CreateUserRequestDTO
            {
                Cpf = "",
                Name = "Nome",
                Email = "email@email.com",
                Telephone = "111",
                Address = "Rua",
                Password = "123"
            };

            _mockService.Setup(s => s.CreateUser(It.IsAny<CreateUserRequestDTO>()))
                        .Returns("CPF é obrigatório.");

            // Act
            var actionResult = _controller.CreateUser(request);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            var bad = (BadRequestObjectResult)actionResult;
            Assert.That(bad.Value, Is.EqualTo("CPF é obrigatório."));
            _mockService.Verify(s => s.CreateUser(It.IsAny<CreateUserRequestDTO>()), Times.Once);
        }

        // ===============================
        // PUT - UpdateUser
        // ===============================

        [Test]
        public void UpdateUser_DeveRetornarNotFound_QuandoNaoEncontrado()
        {
            // Arrange
            string cpf = "123";
            var request = new UpdateUserRequestDTO();

            _mockService.Setup(s => s.UpdateUser(cpf, It.IsAny<UpdateUserRequestDTO>()))
                        .Returns("não encontrado");

            // Act
            var actionResult = _controller.UpdateUser(cpf, request);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<NotFoundObjectResult>());
            var nf = (NotFoundObjectResult)actionResult;
            Assert.That(nf.Value, Is.EqualTo("não encontrado"));
            _mockService.Verify(s => s.UpdateUser(cpf, It.IsAny<UpdateUserRequestDTO>()), Times.Once);
        }

        [Test]
        public void UpdateUser_DeveRetornarOk_QuandoAtualizar()
        {
            // Arrange
            string cpf = "123";
            var request = new UpdateUserRequestDTO();

            _mockService.Setup(s => s.UpdateUser(cpf, It.IsAny<UpdateUserRequestDTO>()))
                        .Returns("ok");

            // Act
            var actionResult = _controller.UpdateUser(cpf, request);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var ok = (OkObjectResult)actionResult;
            Assert.That(ok.Value, Is.EqualTo("ok"));
            _mockService.Verify(s => s.UpdateUser(cpf, It.IsAny<UpdateUserRequestDTO>()), Times.Once);
        }

        // ===============================
        // DELETE - DeleteUser
        // ===============================

        [Test]
        public void DeleteUser_DeveRetornarNotFound_QuandoNaoEncontrado()
        {
            // Arrange
            string cpf = "123";

            _mockService.Setup(s => s.DeleteUser(cpf))
                        .Returns("não encontrado");

            // Act
            var actionResult = _controller.DeleteUser(cpf);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<NotFoundObjectResult>());
            var nf = (NotFoundObjectResult)actionResult;
            Assert.That(nf.Value, Is.EqualTo("não encontrado"));
            _mockService.Verify(s => s.DeleteUser(cpf), Times.Once);
        }

        [Test]
        public void DeleteUser_DeveRetornarOk_QuandoExcluir()
        {
            // Arrange
            string cpf = "123";

            _mockService.Setup(s => s.DeleteUser(cpf))
                        .Returns("ok");

            // Act
            var actionResult = _controller.DeleteUser(cpf);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var ok = (OkObjectResult)actionResult;
            Assert.That(ok.Value, Is.EqualTo("ok"));
            _mockService.Verify(s => s.DeleteUser(cpf), Times.Once);
        }
    }
}
