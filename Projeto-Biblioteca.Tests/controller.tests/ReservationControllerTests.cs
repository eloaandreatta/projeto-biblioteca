using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using pBiblioteca.Controllers;
using pBiblioteca.Services;
using pBiblioteca.Models;
using System.Collections.Generic;
using System.Reflection;

namespace Projeto_Biblioteca.Tests.Controllers
{
    [TestFixture]
    public class ReservationControllerTests
    {
        private Mock<IReservationService> _mockService;
        private ReservationController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IReservationService>();
            _controller = new ReservationController(_mockService.Object);
        }

        // =====================================================
        // POST /Reservation
        // =====================================================

        [Test]
        public void Post_DeveRetornarOk_QuandoServiceRetornarOk()
        {
            var request = new CreateReservationRequest
            {
                UserCpf = "123",
                BookIsbn = "isbn"
            };

            _mockService.Setup(s => s.CreateReservation(request)).Returns("ok");

            var result = _controller.Post(request);

            Assert.That(result, Is.InstanceOf<OkResult>());
            _mockService.Verify(s => s.CreateReservation(request), Times.Once);
        }

        [Test]
        public void Post_DeveRetornarBadRequest_QuandoServiceRetornarDiferenteDeOk()
        {
            var request = new CreateReservationRequest
            {
                UserCpf = "123",
                BookIsbn = "isbn"
            };

            _mockService.Setup(s => s.CreateReservation(request)).Returns("book_not_found");

            var result = _controller.Post(request);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());

            var bad = (BadRequestObjectResult)result;
            Assert.That(bad.Value?.ToString(), Is.EqualTo("book_not_found"));

            _mockService.Verify(s => s.CreateReservation(request), Times.Once);
        }

        // =====================================================
        // DELETE /Reservation/{id}
        // =====================================================

        [Test]
        public void Cancel_DeveRetornarOk_QuandoServiceRetornarOk()
        {
            int id = 10;

            _mockService.Setup(s => s.CancelReservation(id)).Returns("ok");

            var result = _controller.Cancel(id);

            Assert.That(result, Is.InstanceOf<OkResult>());
            _mockService.Verify(s => s.CancelReservation(id), Times.Once);
        }

        [Test]
        public void Cancel_DeveRetornarNotFound_QuandoServiceRetornarNotFound()
        {
            int id = 10;

            _mockService.Setup(s => s.CancelReservation(id)).Returns("not_found");

            var result = _controller.Cancel(id);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            _mockService.Verify(s => s.CancelReservation(id), Times.Once);
        }

        [Test]
        public void Cancel_DeveRetornarBadRequest_QuandoServiceRetornarErro()
        {
            int id = 10;

            _mockService.Setup(s => s.CancelReservation(id)).Returns("cannot_cancel");

            var result = _controller.Cancel(id);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());

            var bad = (BadRequestObjectResult)result;
            Assert.That(bad.Value?.ToString(), Is.EqualTo("cannot_cancel"));

            _mockService.Verify(s => s.CancelReservation(id), Times.Once);
        }

        // =====================================================
        // GET /Reservation/queue/{isbn}
        // =====================================================

        [Test]
        public void GetQueue_DeveRetornarOk_ComLista()
        {
            string isbn = "isbn";

            var queue = new List<ReservationResponseDTO>
            {
                new ReservationResponseDTO { ReservationId = 1, BookIsbn = isbn, UserCpf = "111", Position = 1, Status = true },
                new ReservationResponseDTO { ReservationId = 2, BookIsbn = isbn, UserCpf = "222", Position = 2, Status = true }
            };

            _mockService.Setup(s => s.GetQueue(isbn)).Returns(queue);

            var result = _controller.GetQueue(isbn);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var ok = (OkObjectResult)result;
            Assert.That(ok.Value, Is.InstanceOf<List<ReservationResponseDTO>>());

            var retorno = (List<ReservationResponseDTO>)ok.Value!;
            Assert.That(retorno.Count, Is.EqualTo(2));
            Assert.That(retorno[0].Position, Is.EqualTo(1));

            _mockService.Verify(s => s.GetQueue(isbn), Times.Once);
        }

        [Test]
        public void GetQueue_DeveRetornarOk_ComListaVazia()
        {
            string isbn = "isbn";

            _mockService.Setup(s => s.GetQueue(isbn)).Returns(new List<ReservationResponseDTO>());

            var result = _controller.GetQueue(isbn);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var ok = (OkObjectResult)result;
            Assert.That(ok.Value, Is.InstanceOf<List<ReservationResponseDTO>>());

            var retorno = (List<ReservationResponseDTO>)ok.Value!;
            Assert.That(retorno.Count, Is.EqualTo(0));

            _mockService.Verify(s => s.GetQueue(isbn), Times.Once);
        }

        // =====================================================
        // GET /Reservation/position/{isbn}/{cpf}
        // =====================================================

        [Test]
        public void GetPosition_DeveRetornarNotFound_QuandoServiceRetornarMenos1()
        {
            string isbn = "isbn";
            string cpf = "123";

            _mockService.Setup(s => s.GetUserPosition(isbn, cpf)).Returns(-1);

            var result = _controller.GetPosition(isbn, cpf);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            _mockService.Verify(s => s.GetUserPosition(isbn, cpf), Times.Once);
        }

        [Test]
        public void GetPosition_DeveRetornarOk_ComObjetoPosition()
        {
            string isbn = "isbn";
            string cpf = "123";

            _mockService.Setup(s => s.GetUserPosition(isbn, cpf)).Returns(3);

            var result = _controller.GetPosition(isbn, cpf);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var ok = (OkObjectResult)result;

            // O controller retorna: Ok(new { position = pos });
            // Como é objeto anônimo, lemos via Reflection.
            Assert.That(ok.Value, Is.Not.Null);

            var prop = ok.Value!.GetType().GetProperty("position", BindingFlags.Public | BindingFlags.Instance);
            Assert.That(prop, Is.Not.Null);

            var value = prop!.GetValue(ok.Value);
            Assert.That(value, Is.EqualTo(3));

            _mockService.Verify(s => s.GetUserPosition(isbn, cpf), Times.Once);
        }
    }
}
