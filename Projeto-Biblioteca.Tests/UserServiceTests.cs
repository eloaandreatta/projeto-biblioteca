using NUnit.Framework;
using Moq;
using pBiblioteca.Services;
using pBiblioteca.Repositories;
using pBiblioteca.Models;
using pBiblioteca.DTO;

namespace Projeto_Biblioteca.Tests
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _mockRepository;
        private UserService _service;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IUserRepository>();
            _service = new UserService(_mockRepository.Object);
        }

        // Se usuário existe: Atualiza - Retorna ok
        [Test]
        public void UpdateUser_DeveRetornarOk_QuandoUsuarioExistir()
        {
            // Arrange
            string cpf = "12345678900";

            // configurando o comportamento do mock
            // "Quando GetUserById for chamado com esse CPF, retorne um usuário (ou seja, finja que ele existe no banco)"

            _mockRepository
                .Setup(r => r.GetUserById(cpf))
                .Returns(new TbUser { Cpf = cpf });

            var request = new UpdateUserRequestDTO
            {
                Nome = "Novo Nome",
                Email = "novo@email.com",
                Telefone = "11999999999",
                Endereco = "Novo endereço"
            };

            // Act
            // executamos o método real do serviço
            var resultado = _service.UpdateUser(cpf, request);

            // Assert
            // Verifica se o retorno foi "ok"
            Assert.That(resultado, Is.EqualTo("ok"));

            // Verifica se o método UpdateUserData foi chamado 1 vez
            _mockRepository.Verify(
                r => r.UpdateUserData(cpf, request),
                Times.Once);
        }

        // Se usuário não existe: Não atualiza - Retorna error
        [Test]
        public void UpdateUser_DeveRetornarError_QuandoUsuarioNaoExistir()
        {
            // Arrange
            // "Quando procurar o usuário, retorne null" - Ou seja, finja que ele NÃO existe no banco
            string cpf = "12345678900";

            _mockRepository
                .Setup(r => r.GetUserById(cpf))
                .Returns((TbUser)null);

            var request = new UpdateUserRequestDTO();

            // Act
            var resultado = _service.UpdateUser(cpf, request);

            // Assert
            // Verifica que o método UpdateUserData NUNCA foi chamado
            Assert.That(resultado, Is.EqualTo("error"));

            _mockRepository.Verify(
                r => r.UpdateUserData(It.IsAny<string>(), It.IsAny<UpdateUserRequestDTO>()),
                Times.Never);
        }
    }
}
