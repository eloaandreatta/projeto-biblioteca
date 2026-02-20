using Moq;
using NUnit.Framework;
using pBiblioteca.Models;

[TestFixture]
public class LoanServiceTests
{
    private Mock<ILoanRepository> _repositoryMock = null!;
    private LoanService _service = null!;

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

        _repositoryMock.Verify(x => x.Save(), Times.Once);
    }

    [Test]
    public void GetLoanDetails_SemFiltros_DeveRetornarListaDoRepositorio()
    {
        // Arrange
        var esperado = new List<LoanDetailsDTO>
        {
            new LoanDetailsDTO
            {
                LoanId = 1,
                LoanDate = new DateOnly(2026, 2, 1),
                DueDate = new DateOnly(2026, 2, 15),
                ReturnDate = null,
                UserCpf = "12345678900",
                UserName = "Maria",
                BookIsbn = "9781234567890",
                BookTitle = "Clean Code"
            }
        };

        _repositoryMock
            .Setup(r => r.SelectLoanDetails(null, null, null))
            .Returns(esperado);

        // Act
        var resultado = _service.GetLoanDetails(null, null, null);

        // Assert
        Assert.That(resultado, Is.Not.Null);
        Assert.That(resultado.Count, Is.EqualTo(1));
        Assert.That(resultado[0].UserName, Is.EqualTo("Maria"));

        _repositoryMock.Verify(r => r.SelectLoanDetails(null, null, null), Times.Once);
    }

    [Test]
    public void GetLoanDetails_ComFiltros_DevePassarParametrosParaRepositorio()
    {
        // Arrange
        bool? status = true;
        string userCpf = "12345678900";
        string bookIsbn = "9781234567890";

        _repositoryMock
            .Setup(r => r.SelectLoanDetails(status, userCpf, bookIsbn))
            .Returns(new List<LoanDetailsDTO>());

        // Act
        var resultado = _service.GetLoanDetails(status, userCpf, bookIsbn);

        // Assert
        Assert.That(resultado, Is.Not.Null);
        Assert.That(resultado, Is.Empty);

        _repositoryMock.Verify(r => r.SelectLoanDetails(status, userCpf, bookIsbn), Times.Once);
    }
}