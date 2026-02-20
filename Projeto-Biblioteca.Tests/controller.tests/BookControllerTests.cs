using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using pBiblioteca.Controllers;
using pBiblioteca.Models;
using System.Collections.Generic;

namespace Projeto_Biblioteca.Tests.Controllers
{
    [TestFixture]
    public class BookControllerTests
    {
        private Mock<IBookService> _mockService;
        private BookController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IBookService>();
            _controller = new BookController(_mockService.Object);
        }

        // =====================================================
        // GET /Book
        // =====================================================

        [Test]
        public void Get_DeveRetornarListaDeLivros()
        {
            // Arrange
            var books = new List<BookResponseDTO>
            {
                new BookResponseDTO { Isbn = "1", Title = "A", Author = "X" },
                new BookResponseDTO { Isbn = "2", Title = "B", Author = "Y" }
            };

            _mockService.Setup(s => s.GetBooks()).Returns(books);

            // Act
            var result = _controller.Get();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<List<BookResponseDTO>>());
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Isbn, Is.EqualTo("1"));

            _mockService.Verify(s => s.GetBooks(), Times.Once);
        }

        [Test]
        public void Get_DeveRetornarListaVazia()
        {
            _mockService.Setup(s => s.GetBooks()).Returns(new List<BookResponseDTO>());

            var result = _controller.Get();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
            _mockService.Verify(s => s.GetBooks(), Times.Once);
        }

        // =====================================================
        // GET /Book/{Isbn}
        // =====================================================

        [Test]
        public void GetByIsbn_DeveRetornarNotFound_QuandoNaoExistir()
        {
            _mockService.Setup(s => s.GetBookByIsbn("123"))
                        .Returns((BookResponseDTO?)null);

            var result = _controller.GetByIsbn("123");

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            _mockService.Verify(s => s.GetBookByIsbn("123"), Times.Once);
        }

        [Test]
        public void GetByIsbn_DeveRetornarOk_QuandoExistir()
        {
            var dto = new BookResponseDTO
            {
                Isbn = "123",
                Title = "Livro",
                Author = "Autor"
            };

            _mockService.Setup(s => s.GetBookByIsbn("123")).Returns(dto);

            var result = _controller.GetByIsbn("123");

            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var ok = (OkObjectResult)result;
            Assert.That(ok.Value, Is.InstanceOf<BookResponseDTO>());

            var value = (BookResponseDTO)ok.Value!;
            Assert.That(value.Isbn, Is.EqualTo("123"));
            Assert.That(value.Title, Is.EqualTo("Livro"));

            _mockService.Verify(s => s.GetBookByIsbn("123"), Times.Once);
        }

        // =====================================================
        // POST /Book
        // =====================================================

        [Test]
        public void Post_DeveRetornarBadRequest_QuandoServiceRetornarError()
        {
            var request = new CreateBookRequest
            {
                Isbn = "",
                Title = "Livro",
                Author = "Autor",
                PublicationYear = 2024,
                Category = "Cat",
                Publisher = "Pub",
                TotalQuantity = 1,
                AvailableQuantity = 1
            };

            _mockService.Setup(s => s.CreateBook(It.IsAny<CreateBookRequest>()))
                        .Returns("error");

            var result = _controller.Post(request);

            Assert.That(result, Is.InstanceOf<BadRequestResult>());

            _mockService.Verify(s => s.CreateBook(It.IsAny<CreateBookRequest>()), Times.Once);
        }

        [Test]
        public void Post_DeveRetornarOk_QuandoServiceRetornarSucesso()
        {
            var request = new CreateBookRequest
            {
                Isbn = "123",
                Title = "Livro",
                Author = "Autor",
                PublicationYear = 2024,
                Category = "Cat",
                Publisher = "Pub",
                TotalQuantity = 1,
                AvailableQuantity = 1
            };

            _mockService.Setup(s => s.CreateBook(It.IsAny<CreateBookRequest>()))
                        .Returns(""); 

            var result = _controller.Post(request);

            Assert.That(result, Is.InstanceOf<OkResult>());

            _mockService.Verify(s => s.CreateBook(It.IsAny<CreateBookRequest>()), Times.Once);
        }
    }
}
