using NUnit.Framework;
using Moq;
using pBiblioteca.Models;
using pBiblioteca.DTO;
using System;
using System.Collections.Generic;

namespace Projeto_Biblioteca.Tests
{
    [TestFixture]
    public class FineServiceTests
    {
        private Mock<IFineRepository> _mockFineRepository;
        private Mock<ILoanRepository> _mockLoanRepository;
        private FineService _service;

        [SetUp]
        public void Setup()
        {
            _mockFineRepository = new Mock<IFineRepository>();
            _mockLoanRepository = new Mock<ILoanRepository>();

            _service = new FineService(
                _mockFineRepository.Object,
                _mockLoanRepository.Object);
        }

        // =====================================================
        // PAY FINE
        // =====================================================

        [Test]
        public void PayFine_DeveRetornarError_QuandoIdForInvalido()
        {
            var resultado = _service.PayFine(0);

            Assert.That(resultado, Is.EqualTo("error"));

            _mockFineRepository.Verify(
                r => r.PayFine(It.IsAny<int>(), It.IsAny<DateOnly>()),
                Times.Never);
        }

        [Test]
        public void PayFine_DeveRetornarError_QuandoMultaNaoExistir()
        {
            int fineId = 999;

            _mockFineRepository
                .Setup(r => r.GetFineById(fineId))
                .Returns((TbFine?)null);

            var resultado = _service.PayFine(fineId);

            Assert.That(resultado, Is.EqualTo("error"));

            _mockFineRepository.Verify(
                r => r.PayFine(It.IsAny<int>(), It.IsAny<DateOnly>()),
                Times.Never);
        }

        [Test]
        public void PayFine_DeveRetornarError_QuandoMultaJaEstiverPaga()
        {
            int fineId = 2;

            _mockFineRepository
                .Setup(r => r.GetFineById(fineId))
                .Returns(new TbFine { Id = fineId, Ispaid = true });

            var resultado = _service.PayFine(fineId);

            Assert.That(resultado, Is.EqualTo("error"));

            _mockFineRepository.Verify(
                r => r.PayFine(It.IsAny<int>(), It.IsAny<DateOnly>()),
                Times.Never);
        }

        [Test]
        public void PayFine_DeveRetornarOk_QuandoMultaExistirENaoEstiverPaga()
        {
            int fineId = 1;

            _mockFineRepository
                .Setup(r => r.GetFineById(fineId))
                .Returns(new TbFine { Id = fineId, Ispaid = false });

            _mockFineRepository
                .Setup(r => r.PayFine(fineId, It.IsAny<DateOnly>()))
                .Returns(true);

            var resultado = _service.PayFine(fineId);

            Assert.That(resultado, Is.EqualTo(""));

            _mockFineRepository.Verify(
                r => r.PayFine(fineId, It.IsAny<DateOnly>()),
                Times.Once);
        }

        [Test]
        public void PayFine_DeveRetornarError_QuandoRepositorioFalhar()
        {
            int fineId = 3;

            _mockFineRepository
                .Setup(r => r.GetFineById(fineId))
                .Returns(new TbFine { Id = fineId, Ispaid = false });

            _mockFineRepository
                .Setup(r => r.PayFine(fineId, It.IsAny<DateOnly>()))
                .Returns(false);

            var resultado = _service.PayFine(fineId);

            Assert.That(resultado, Is.EqualTo("error"));

            _mockFineRepository.Verify(
                r => r.PayFine(fineId, It.IsAny<DateOnly>()),
                Times.Once);
        }

        // =====================================================
        // HAS OPEN FINE
        // =====================================================

        [Test]
        public void HasOpenFineByCpf_DeveRetornarFalse_QuandoCpfVazio()
        {
            var resultado = _service.HasOpenFineByCpf("");

            Assert.That(resultado, Is.False);

            _mockFineRepository.Verify(
                r => r.HasOpenFineByCpf(It.IsAny<string>()),
                Times.Never);
        }

        [Test]
        public void HasOpenFineByCpf_DeveRetornarTrue_QuandoRepositorioIndicarMulta()
        {
            string cpf = "12345678900";

            _mockFineRepository
                .Setup(r => r.HasOpenFineByCpf(cpf))
                .Returns(true);

            var resultado = _service.HasOpenFineByCpf(cpf);

            Assert.That(resultado, Is.True);

            _mockFineRepository.Verify(
                r => r.HasOpenFineByCpf(cpf),
                Times.Once);
        }

        [Test]
        public void HasOpenFineByCpf_DeveRetornarFalse_QuandoRepositorioNaoIndicarMulta()
        {
            string cpf = "12345678900";

            _mockFineRepository
                .Setup(r => r.HasOpenFineByCpf(cpf))
                .Returns(false);

            var resultado = _service.HasOpenFineByCpf(cpf);

            Assert.That(resultado, Is.False);

            _mockFineRepository.Verify(
                r => r.HasOpenFineByCpf(cpf),
                Times.Once);
        }

        // =====================================================
        // GET FINES BY CPF
        // =====================================================

        [Test]
        public void GetFinesByCpf_DeveRetornarListaVazia_QuandoCpfVazio()
        {
            var resultado = _service.GetFinesByCpf("");

            Assert.That(resultado.Count, Is.EqualTo(0));

            _mockFineRepository.Verify(
                r => r.SelectFinesByCpf(It.IsAny<string>()),
                Times.Never);
        }

        [Test]
        public void GetFinesByCpf_DeveMapearDTO_Corretamente()
        {
            string cpf = "12345678900";

            var fines = new List<TbFine>
            {
                new TbFine
                {
                    Id = 1,
                    UserCpf = cpf,
                    LoanId = 5,
                    Amount = 10.50m,
                    Dayslate = 35,
                    Ispaid = false,
                    Dailyrate = 0.30m,
                    Paymentdate = DateOnly.FromDateTime(DateTime.MinValue)
                }
            };

            _mockFineRepository
                .Setup(r => r.SelectFinesByCpf(cpf))
                .Returns(fines);

            var resultado = _service.GetFinesByCpf(cpf);

            Assert.That(resultado.Count, Is.EqualTo(1));
            Assert.That(resultado[0].LoanId, Is.EqualTo(5));
            Assert.That(resultado[0].Amount, Is.EqualTo(10.50m));
        }

        // =====================================================
        // GET FINE BY ID
        // =====================================================

        [Test]
        public void GetFineById_DeveRetornarNull_QuandoIdInvalido()
        {
            var resultado = _service.GetFineById(0);

            Assert.That(resultado, Is.Null);

            _mockFineRepository.Verify(
                r => r.GetFineById(It.IsAny<int>()),
                Times.Never);
        }

        [Test]
        public void GetFineById_DeveRetornarNull_QuandoNaoExistir()
        {
            int id = 10;

            _mockFineRepository
                .Setup(r => r.GetFineById(id))
                .Returns((TbFine?)null);

            var resultado = _service.GetFineById(id);

            Assert.That(resultado, Is.Null);
        }

        [Test]
        public void GetFineById_DeveMapearDTO_Corretamente()
        {
            int id = 1;

            _mockFineRepository
                .Setup(r => r.GetFineById(id))
                .Returns(new TbFine
                {
                    Id = id,
                    UserCpf = "123",
                    LoanId = 5,
                    Amount = 12.30m,
                    Dayslate = 41,
                    Ispaid = false,
                    Dailyrate = 0.30m,
                    Paymentdate = DateOnly.FromDateTime(DateTime.MinValue)
                });

            var resultado = _service.GetFineById(id);

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado!.Id, Is.EqualTo(id));
            Assert.That(resultado.Amount, Is.EqualTo(12.30m));
        }
    }
}
