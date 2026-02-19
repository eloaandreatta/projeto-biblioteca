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
        // Arrange
        var request = new CreateLoanRequest
        {
            UserCpf = "12345678900",
            BookIsbn = "9781234567890"
        };

        _repositoryMock.Setup(x => x.GetUserByCpf(request.UserCpf))
                    .Returns(new TbUser { Cpf = request.UserCpf, Active = true });

        _repositoryMock.Setup(x => x.UserHasActiveLoan(request.UserCpf)).Returns(false);
        _repositoryMock.Setup(x => x.UserHasUnpaidFine(request.UserCpf)).Returns(false);

        _repositoryMock.Setup(x => x.GetBookByIsbn(request.BookIsbn))
                    .Returns(new TbBook { Isbn = request.BookIsbn, Availablequantity = 1 });

        _repositoryMock.Setup(x => x.InsertLoan(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<DateOnly>(),
            It.IsAny<DateOnly>()))
            .Returns(true);

        // Act
        var resultado = _service.CreateLoan(request);

        // Assert
        Assert.That(resultado, Is.EqualTo(""));

        _repositoryMock.Verify(x => x.InsertLoan(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<DateOnly>(),
            It.IsAny<DateOnly>()), Times.Once);
    }

}