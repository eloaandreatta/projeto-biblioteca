using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using pBiblioteca.Controllers;
using pBiblioteca.Models;
using System.Collections.Generic;

namespace Projeto_Biblioteca.Tests.Controllers
{
    [TestFixture]
    public class LoanControllerTests
    {
        private Mock<ILoanService> _mockService;
        private LoanController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<ILoanService>();
            _controller = new LoanController(_mockService.Object);
        }

        // =====================================================
        // GET /Loan  (GetAllLoans)
        // =====================================================

        [Test]
        public void Get_DeveRetornarOk_ComListaDeLoans()
        {
            var loans = new List<LoanResponseDTO>
            {
                new LoanResponseDTO { Id = 1, UserCpf = "123", BookIsbn = "isbn1" },
                new LoanResponseDTO { Id = 2, UserCpf = "456", BookIsbn = "isbn2" }
            };

            _mockService.Setup(s => s.GetLoans()).Returns(loans);

            var result = _controller.Get();

            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var ok = (OkObjectResult)result;
            Assert.That(ok.Value, Is.InstanceOf<List<LoanResponseDTO>>());

            var value = (List<LoanResponseDTO>)ok.Value!;
            Assert.That(value.Count, Is.EqualTo(2));
            Assert.That(value[0].Id, Is.EqualTo(1));

            _mockService.Verify(s => s.GetLoans(), Times.Once);
        }

        // =====================================================
        // GET /Loan/{id}
        // =====================================================

        [Test]
        public void GetById_DeveRetornarNotFound_QuandoNaoExistir()
        {
            _mockService.Setup(s => s.GetLoanById(10)).Returns((LoanResponseDTO?)null);

            var result = _controller.GetById(10);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            _mockService.Verify(s => s.GetLoanById(10), Times.Once);
        }

        [Test]
        public void GetById_DeveRetornarOk_QuandoExistir()
        {
            var dto = new LoanResponseDTO { Id = 1, UserCpf = "123", BookIsbn = "isbn" };

            _mockService.Setup(s => s.GetLoanById(1)).Returns(dto);

            var result = _controller.GetById(1);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = (OkObjectResult)result;

            Assert.That(ok.Value, Is.InstanceOf<LoanResponseDTO>());
            var value = (LoanResponseDTO)ok.Value!;
            Assert.That(value.Id, Is.EqualTo(1));

            _mockService.Verify(s => s.GetLoanById(1), Times.Once);
        }

        // =====================================================
        // GET /Loan/user/{cpf}
        // =====================================================

        [Test]
        public void GetByUser_DeveRetornarNotFound_QuandoListaVazia()
        {
            string cpf = "123";
            _mockService.Setup(s => s.GetLoansByUser(cpf))
                        .Returns(new List<LoanResponseDTO>());

            var result = _controller.GetByUser(cpf);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            _mockService.Verify(s => s.GetLoansByUser(cpf), Times.Once);
        }

        [Test]
        public void GetByUser_DeveRetornarOk_QuandoTiverLoans()
        {
            string cpf = "123";
            _mockService.Setup(s => s.GetLoansByUser(cpf))
                        .Returns(new List<LoanResponseDTO>
                        {
                            new LoanResponseDTO { Id = 1, UserCpf = cpf, BookIsbn = "isbn" }
                        });

            var result = _controller.GetByUser(cpf);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = (OkObjectResult)result;

            Assert.That(ok.Value, Is.InstanceOf<List<LoanResponseDTO>>());
            var list = (List<LoanResponseDTO>)ok.Value!;
            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list[0].UserCpf, Is.EqualTo(cpf));

            _mockService.Verify(s => s.GetLoansByUser(cpf), Times.Once);
        }

        // =====================================================
        // POST /Loan
        // =====================================================

        [Test]
        public void Post_DeveRetornarBadRequest_QuandoServiceRetornarError()
        {
            var request = new CreateLoanRequest { UserCpf = "123", BookIsbn = "isbn" };

            _mockService.Setup(s => s.CreateLoan(It.IsAny<CreateLoanRequest>()))
                        .Returns("error");

            var result = _controller.Post(request);

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
            _mockService.Verify(s => s.CreateLoan(It.IsAny<CreateLoanRequest>()), Times.Once);
        }

        [Test]
        public void Post_DeveRetornarCreated_QuandoServiceRetornarSucesso()
        {
            var request = new CreateLoanRequest { UserCpf = "123", BookIsbn = "isbn" };

            _mockService.Setup(s => s.CreateLoan(It.IsAny<CreateLoanRequest>()))
                        .Returns(""); // sucesso

            var result = _controller.Post(request);

            Assert.That(result, Is.InstanceOf<CreatedResult>());
            _mockService.Verify(s => s.CreateLoan(It.IsAny<CreateLoanRequest>()), Times.Once);
        }

        // =====================================================
        // PUT /Loan/return/{id}
        // =====================================================

        [Test]
        public void ReturnLoan_DeveRetornarBadRequest_QuandoServiceRetornarError()
        {
            _mockService.Setup(s => s.ReturnLoan(1)).Returns("error");

            var result = _controller.ReturnLoan(1);

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
            _mockService.Verify(s => s.ReturnLoan(1), Times.Once);
        }

        [Test]
        public void ReturnLoan_DeveRetornarOk_QuandoServiceRetornarSucesso()
        {
            _mockService.Setup(s => s.ReturnLoan(1)).Returns("");

            var result = _controller.ReturnLoan(1);

            Assert.That(result, Is.InstanceOf<OkResult>());
            _mockService.Verify(s => s.ReturnLoan(1), Times.Once);
        }

        // =====================================================
        // PUT /Loan/renew/{id}
        // =====================================================

        [Test]
        public void RenewLoan_DeveRetornarBadRequest_QuandoServiceRetornarError()
        {
            _mockService.Setup(s => s.RenewLoan(1)).Returns("error");

            var result = _controller.RenewLoan(1);

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
            _mockService.Verify(s => s.RenewLoan(1), Times.Once);
        }

        [Test]
        public void RenewLoan_DeveRetornarOk_QuandoServiceRetornarOk()
        {
            _mockService.Setup(s => s.RenewLoan(1)).Returns("ok");

            var result = _controller.RenewLoan(1);

            Assert.That(result, Is.InstanceOf<OkResult>());
            _mockService.Verify(s => s.RenewLoan(1), Times.Once);
        }
    }
}
