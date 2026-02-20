using Moq;
using NUnit.Framework;
using pBiblioteca.Models;
using System.Collections.Generic;
<<<<<<< HEAD

namespace pBiblioteca.Services
=======
using pBiblioteca.Services;
namespace Projeto_Biblioteca.Tests
>>>>>>> 79e4bf33068926dc124bbbf5480521bd24b04751
{
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

        // ===============================
        // GET BOOKS
        // ===============================

        [Test]
        public void GetBooks_WhenNoBooks_ShouldReturnEmptyList()
        {
            _repoMock.Setup(r => r.SelectBooks()).Returns(new List<TbBook>());

            var result = _service.GetBooks();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetBooks_WhenHasBooks_ShouldMapCorrectly()
        {
            var books = new List<TbBook>
            {
                new TbBook { Isbn="1", Title="A", Author="X", Availablequantity=1, Totalquantity=2 }
            };

            _repoMock.Setup(r => r.SelectBooks()).Returns(books);

            var result = _service.GetBooks();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Isbn, Is.EqualTo("1"));
            Assert.That(result[0].Title, Is.EqualTo("A"));
        }

        // ===============================
        // GET BOOK BY ISBN
        // ===============================

        [Test]
        public void GetBookByIsbn_WhenIsbnEmpty_ShouldReturnNull()
        {
            var result = _service.GetBookByIsbn("");

            Assert.That(result, Is.Null);
            _repoMock.Verify(r => r.GetBookByIsbn(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GetBookByIsbn_WhenNotFound_ShouldReturnNull()
        {
            _repoMock.Setup(r => r.GetBookByIsbn("123"))
                     .Returns((TbBook?)null);

            var result = _service.GetBookByIsbn("123");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetBookByIsbn_WhenFound_ShouldMapDTO()
        {
            _repoMock.Setup(r => r.GetBookByIsbn("123"))
                     .Returns(new TbBook
                     {
                         Isbn = "123",
                         Title = "Livro",
                         Author = "Autor",
                         Publicationyear = 2024,
                         Category = "Cat",
                         Publisher = "Pub",
                         Totalquantity = 10,
                         Availablequantity = 5
                     });

            var result = _service.GetBookByIsbn("123");

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.AvailableQuantity, Is.EqualTo(5));
        }

        // ===============================
        // CREATE BOOK
        // ===============================

        [Test]
        public void CreateBook_WhenRequestNull_ShouldReturnError()
        {
            var result = _service.CreateBook(null!);

            Assert.That(result, Is.EqualTo("error"));
        }

        [Test]
        public void CreateBook_WhenIsbnEmpty_ShouldReturnError()
        {
            var result = _service.CreateBook(new CreateBookRequest
            {
                Isbn = "",
                Title = "T",
                Author = "A",
                Category = "C",
                Publisher = "P",
                PublicationYear = 2024,
                TotalQuantity = 1,
                AvailableQuantity = 1
            });

            Assert.That(result, Is.EqualTo("error"));
        }

        [Test]
        public void CreateBook_WhenAvailableGreaterThanTotal_ShouldReturnError()
        {
            var result = _service.CreateBook(new CreateBookRequest
            {
                Isbn = "123",
                Title = "T",
                Author = "A",
                Category = "C",
                Publisher = "P",
                PublicationYear = 2024,
                TotalQuantity = 1,
                AvailableQuantity = 5 // ❌ maior que total
            });

            Assert.That(result, Is.EqualTo("error"));
            _repoMock.Verify(r => r.InsertBook(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()
            ), Times.Never);
        }

        [Test]
        public void CreateBook_WhenIsbnDuplicated_ShouldReturnError()
        {
            _repoMock.Setup(r => r.GetBookByIsbn("123"))
                     .Returns(new TbBook { Isbn = "123" });

            var result = _service.CreateBook(new CreateBookRequest
            {
                Isbn = "123",
                Title = "T",
                Author = "A",
                Category = "C",
                Publisher = "P",
                PublicationYear = 2024,
                TotalQuantity = 1,
                AvailableQuantity = 1
            });

            Assert.That(result, Is.EqualTo("error"));
        }

        [Test]
        public void CreateBook_WhenValid_ShouldInsertAndReturnSuccess()
        {
            _repoMock.Setup(r => r.GetBookByIsbn("123"))
                     .Returns((TbBook?)null);

            var result = _service.CreateBook(new CreateBookRequest
            {
                Isbn = "123",
                Title = "Livro",
                Author = "Autor",
                Category = "Cat",
                Publisher = "Pub",
                PublicationYear = 2024,
                TotalQuantity = 10,
                AvailableQuantity = 10
            });

            Assert.That(result, Is.EqualTo(""));

            _repoMock.Verify(r => r.InsertBook(
                "123",
                "Livro",
                "Autor",
                2024,
                "Cat",
                "Pub",
                10,
                10
            ), Times.Once);
        }
    }
}
