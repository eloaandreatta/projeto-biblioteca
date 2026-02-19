using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using pBiblioteca.Controllers;
using pBiblioteca.DTO;
using System.Collections.Generic;

namespace Projeto_Biblioteca.Tests.Controllers
{
    [TestFixture]
    public class FineControllerTests
    {
        private Mock<IFineService> _mockService;
        private FineController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IFineService>();
            _controller = new FineController(_mockService.Object);
        }

        // =====================================================
        // GET /Fine/user/{cpf}
        // =====================================================

        [Test]
        public void GetByCpf_DeveRetornarOk_ComListaDeMultas()
        {
            string cpf = "12345678900";

            var retorno = new List<FineResponseDTO>
            {
                new FineResponseDTO
                {
                    Id = 1,
                    UserCpf = cpf,
                    LoanId = 5,
                    Amount = 10.0m,
                    DaysLate = 10,
                    IsPaid = false,
                    DailyRate = 0.30m,
                    PaymentDate = default
                }
            };

            _mockService
                .Setup(s => s.GetFinesByCpf(cpf))
                .Returns(retorno);

            var result = _controller.GetByCpf(cpf);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var ok = (OkObjectResult)result.Result!;
            Assert.That(ok.Value, Is.InstanceOf<List<FineResponseDTO>>());

            var lista = (List<FineResponseDTO>)ok.Value!;
            Assert.That(lista.Count, Is.EqualTo(1));
            Assert.That(lista[0].Id, Is.EqualTo(1));

            _mockService.Verify(s => s.GetFinesByCpf(cpf), Times.Once);
        }

        [Test]
        public void GetByCpf_DeveRetornarOk_ComListaVazia()
        {
            string cpf = "12345678900";

            _mockService
                .Setup(s => s.GetFinesByCpf(cpf))
                .Returns(new List<FineResponseDTO>());

            var result = _controller.GetByCpf(cpf);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var ok = (OkObjectResult)result.Result!;
            var lista = (List<FineResponseDTO>)ok.Value!;

            Assert.That(lista, Is.Not.Null);
            Assert.That(lista.Count, Is.EqualTo(0));

            _mockService.Verify(s => s.GetFinesByCpf(cpf), Times.Once);
        }

        // =====================================================
        // GET /Fine/loan/{loanId}
        // =====================================================

        [Test]
        public void GetByLoan_DeveRetornarNotFound_QuandoServiceRetornarNull()
        {
            int loanId = 5;

            _mockService
                .Setup(s => s.GetOrCreateFineByLoanId(loanId))
                .Returns((FineResponseDTO?)null);

            var result = _controller.GetByLoan(loanId);

            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());

            _mockService.Verify(s => s.GetOrCreateFineByLoanId(loanId), Times.Once);
        }

        [Test]
        public void GetByLoan_DeveRetornarOk_QuandoServiceRetornarMulta()
        {
            int loanId = 5;

            var fine = new FineResponseDTO
            {
                Id = 1,
                UserCpf = "12345678900",
                LoanId = loanId,
                Amount = 3.00m,
                DaysLate = 10,
                IsPaid = false,
                DailyRate = 0.30m,
                PaymentDate = default
            };

            _mockService
                .Setup(s => s.GetOrCreateFineByLoanId(loanId))
                .Returns(fine);

            var result = _controller.GetByLoan(loanId);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var ok = (OkObjectResult)result.Result!;
            Assert.That(ok.Value, Is.InstanceOf<FineResponseDTO>());

            var retorno = (FineResponseDTO)ok.Value!;
            Assert.That(retorno.LoanId, Is.EqualTo(loanId));
            Assert.That(retorno.Amount, Is.EqualTo(3.00m));

            _mockService.Verify(s => s.GetOrCreateFineByLoanId(loanId), Times.Once);
        }

        // =====================================================
        // POST /Fine/{id}/pay
        // =====================================================

        [Test]
        public void Pay_DeveRetornarBadRequest_QuandoServiceRetornarError()
        {
            int fineId = 5;

            _mockService
                .Setup(s => s.PayFine(fineId))
                .Returns("error");

            var result = _controller.Pay(fineId);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());

            var bad = (BadRequestObjectResult)result;
            Assert.That(bad.Value, Is.EqualTo("Multa não encontrada ou já paga."));

            _mockService.Verify(s => s.PayFine(fineId), Times.Once);
        }

        [Test]
        public void Pay_DeveRetornarOk_QuandoServiceRetornarSucesso()
        {
            int fineId = 5;

            _mockService
                .Setup(s => s.PayFine(fineId))
                .Returns("");

            var result = _controller.Pay(fineId);

            Assert.That(result, Is.InstanceOf<OkResult>());

            _mockService.Verify(s => s.PayFine(fineId), Times.Once);
        }
    }
}
