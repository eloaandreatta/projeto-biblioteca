using NUnit.Framework;
using Moq;
using pBiblioteca.Models;

[TestFixture]
public class LoanServiceTests
{
    private Mock<ILoanRepository> _repositoryMock;
    private LoanService _service;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<ILoanRepository>();
        _service = new LoanService(_repositoryMock.Object);
    }

    [Test]
    public void CreateLoan_DadosValidos_DeveRetornarSucesso()
    {
        // Arrange -> Preparação
        // Criamos um request válido para simular a criação de empréstimo
        var request = new CreateLoanRequest
        {
            UserCpf = "12345678900",
            BookIsbn = "9781234567890",
            DueDate = DateTime.Now.AddDays(7)
        };

        // Configuramos o mock:
        // Quando InsertLoan for chamado com qualquer parametro válido, ele deve retornar true (simulando sucesso no banco)
        _repositoryMock
            .Setup(x => x.InsertLoan(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>()))
            .Returns(true);

        // Act -> Execução
        var resultado = _service.CreateLoan(request);

        // Assert
        // Verifica se o retorno foi string vazia, que no caso significa sucesso
        Assert.That(resultado, Is.EqualTo(""));
        // Verificamos se o método InsertLoan do repository foi chamado exatamente 1 vez
        // giIsso garante que a service realmente tentou salvar no banco
        _repositoryMock.Verify(
        x => x.InsertLoan(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<DateOnly>(),
            It.IsAny<DateOnly>()),
        Times.Once);

    }
}