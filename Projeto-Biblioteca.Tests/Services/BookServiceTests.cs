using Moq;
using NUnit.Framework;
using pBiblioteca.Models;
using Projeto_Biblioteca.Tests.Fixtures.TestData;

namespace Projeto_Biblioteca.Tests.Services;

[TestFixture]
public class BookServiceTests
{
    private Mock<IBookRepository> _repo = null!;
    private BookService _service = null!;

    [SetUp]
    public void Setup()
    {
        _repo = new Mock<IBookRepository>(MockBehavior.Strict);
        _service = new BookService(_repo.Object);
    }

    [Test]
    public void GetBooks_QuandoVazio_DeveRetornarListaVazia()
    {
        _repo.Setup(r => r.SelectBooks()).Returns(new List<TbBook>());

        var result = _service.GetBooks();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(0));
        _repo.Verify(r => r.SelectBooks(), Times.Once);
    }

    [Test]
    public void GetBooks_QuandoTemLivros_DeveMapearDTO()
    {
        _repo.Setup(r => r.SelectBooks()).Returns(new List<TbBook>
        {
            Factory.Book(isbn: "1", available: 1, total: 2),
            Factory.Book(isbn: "2", available: 0, total: 1),
        });

        var result = _service.GetBooks();

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].Isbn, Is.EqualTo("1"));
        Assert.That(result[1].Isbn, Is.EqualTo("2"));
        _repo.Verify(r => r.SelectBooks(), Times.Once);
    }

    [Test]
    public void GetBookByIsbn_QuandoIsbnVazio_DeveRetornarNull()
    {
        var result = _service.GetBookByIsbn("   ");
        Assert.That(result, Is.Null);
        _repo.Verify(r => r.GetBookByIsbn(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetBookByIsbn_QuandoNaoExiste_DeveRetornarNull()
    {
        _repo.Setup(r => r.GetBookByIsbn("978")).Returns((TbBook?)null);

        var result = _service.GetBookByIsbn(" 978 ");

        Assert.That(result, Is.Null);
        _repo.Verify(r => r.GetBookByIsbn("978"), Times.Once);
    }

    [Test]
    public void GetBookByIsbn_QuandoExiste_DeveMapearDTO()
    {
        _repo.Setup(r => r.GetBookByIsbn("978"))
            .Returns(Factory.Book(isbn: "978", available: 3, total: 10));

        var result = _service.GetBookByIsbn(" 978 ");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Isbn, Is.EqualTo("978"));
        Assert.That(result.AvailableQuantity, Is.EqualTo(3));
        _repo.Verify(r => r.GetBookByIsbn("978"), Times.Once);
    }

    [Test]
    public void CreateBook_QuandoRequestNull_DeveRetornarError()
    {
        var result = _service.CreateBook(null!);

        Assert.That(result, Is.EqualTo("error"));
        _repo.Verify(r => r.GetBookByIsbn(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void CreateBook_QuandoCamposObrigatoriosInvalidos_DeveRetornarError()
    {
        var req = Factory.CreateBookRequest(isbn: " ");
        Assert.That(_service.CreateBook(req), Is.EqualTo("error"));

        req = Factory.CreateBookRequest(title: " ");
        Assert.That(_service.CreateBook(req), Is.EqualTo("error"));

        req = Factory.CreateBookRequest(author: " ");
        Assert.That(_service.CreateBook(req), Is.EqualTo("error"));

        req = Factory.CreateBookRequest(category: " ");
        Assert.That(_service.CreateBook(req), Is.EqualTo("error"));

        req = Factory.CreateBookRequest(publisher: " ");
        Assert.That(_service.CreateBook(req), Is.EqualTo("error"));

        _repo.Verify(r => r.GetBookByIsbn(It.IsAny<string>()), Times.Never);
        _repo.Verify(r => r.InsertBook(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }

    [Test]
    public void CreateBook_QuandoValoresNumericosInvalidos_DeveRetornarError()
    {
        var req = Factory.CreateBookRequest(publicationYear: 0);
        Assert.That(_service.CreateBook(req), Is.EqualTo("error"));

        req = Factory.CreateBookRequest(totalQuantity: -1);
        Assert.That(_service.CreateBook(req), Is.EqualTo("error"));

        req = Factory.CreateBookRequest(availableQuantity: -1);
        Assert.That(_service.CreateBook(req), Is.EqualTo("error"));

        _repo.Verify(r => r.GetBookByIsbn(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void CreateBook_QuandoIsbnJaExiste_DeveRetornarError()
    {
        var req = Factory.CreateBookRequest(isbn: " 978 ");
        _repo.Setup(r => r.GetBookByIsbn("978")).Returns(Factory.Book(isbn: "978"));

        var result = _service.CreateBook(req);

        Assert.That(result, Is.EqualTo("error"));
        _repo.Verify(r => r.InsertBook(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }

    [Test]
    public void CreateBook_QuandoOk_DeveInserirERetornarStringVazia()
    {
        var req = Factory.CreateBookRequest(
            isbn: " 978 ",
            title: " T ",
            author: " A ",
            category: " C ",
            publisher: " P ",
            totalQuantity: 10,
            availableQuantity: 5
        );

        _repo.Setup(r => r.GetBookByIsbn("978")).Returns((TbBook?)null);
        _repo.Setup(r => r.InsertBook("978", "T", "A", req.PublicationYear, "C", "P", req.TotalQuantity, req.AvailableQuantity));

        var result = _service.CreateBook(req);

        Assert.That(result, Is.EqualTo(""));
        _repo.VerifyAll();
    }
}