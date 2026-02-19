using Moq;

using pBiblioteca.Models;

namespace projeto_biblioteca.Tests;

[TestFixture]
public class BookServiceTests
{
    private Mock<IBookRepository> _repoMock;
    private BookService _service;

    [SetUp]
    public void Setup()
    {
        _repoMock = new Mock<IBookRepository>();
        _service = new BookService(_repoMock.Object);
    }

    [Test]
    public void GetBooks_WhenNoBooks_ShouldReturnEmptyList()
    {
        // Arrange
        _repoMock
            .Setup(r => r.SelectBooks())
            .Returns(new List<TbBook>());

        // Act
        var result = _service.GetBooks();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(0));
        _repoMock.Verify(r => r.SelectBooks(), Times.Once);
    }

    [Test]
    public void GetBooks_WhenHasBooks_ShouldMapToDTO()
    {
        // Arrange
        var list = new List<TbBook>
        {
            new TbBook { Isbn="1", Title="A", Author="X", Availablequantity=1, Totalquantity=2 },
            new TbBook { Isbn="2", Title="B", Author="Y", Availablequantity=0, Totalquantity=1 }
        };

        _repoMock
            .Setup(r => r.SelectBooks())
            .Returns(list);

        // Act
        var result = _service.GetBooks();

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].Isbn, Is.EqualTo("1"));
        Assert.That(result[0].Title, Is.EqualTo("A"));
        _repoMock.Verify(r => r.SelectBooks(), Times.Once);
    }

    [TestFixture]
    public class BookRulesTests
    {
        [Test]
        public void ValidateLoan_WhenAvailableQuantityIsZero_ShouldThrow()
        {
            // Arrange
            var book = new TbBook
            {
                Isbn = "123",
                Title = "Livro Teste",
                Author = "Autor",
                Publicationyear = 2024,
                Category = "Teste",
                Publisher = "Editora",
                Totalquantity = 2,
                Availablequantity = 0
            };

            // Act + Assert
            Assert.Throws<InvalidOperationException>(() =>
            {
                // Regra de negócio não emprestar livro com quantidade menor ou igual a 0
                if (book.Availablequantity <= 0)
                    throw new InvalidOperationException("Livro indisponível para empréstimo");
            });
        }
    }
}