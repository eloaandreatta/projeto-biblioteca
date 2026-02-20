using NUnit.Framework;
using Moq;
using pBiblioteca.Models;
using System;
using System.Collections.Generic;
using pBiblioteca.Services;

namespace pBiblioteca.Services
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
        // GET OR CREATE FINE BY LOAN ID  ✅ (faltava)
        // =====================================================

        [Test]
        public void GetOrCreateFineByLoanId_DeveRetornarNull_QuandoLoanIdInvalido()
        {
            var result = _service.GetOrCreateFineByLoanId(0);

            Assert.That(result, Is.Null);
            _mockLoanRepository.Verify(r => r.GetLoanById(It.IsAny<int>()), Times.Never);
            _mockFineRepository.Verify(r => r.GetFineByLoanId(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void GetOrCreateFineByLoanId_DeveRetornarNull_QuandoLoanNaoExiste()
        {
            int loanId = 10;

            _mockLoanRepository.Setup(r => r.GetLoanById(loanId))
                               .Returns((TbLoan?)null);

            var result = _service.GetOrCreateFineByLoanId(loanId);

            Assert.That(result, Is.Null);
            _mockLoanRepository.Verify(r => r.GetLoanById(loanId), Times.Once);
            _mockFineRepository.Verify(r => r.GetFineByLoanId(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void GetOrCreateFineByLoanId_DeveRetornarNull_QuandoNaoHaAtraso()
        {
            int loanId = 5;

            // Duedate >= endDate => daysLate <= 0 => sem multa
            var loan = new TbLoan
            {
                Id = loanId,
                UserCpf = "123",
                Duedate = DateOnly.FromDateTime(DateTime.Now),
                Returndate = DateOnly.FromDateTime(DateTime.Now) // devolveu no prazo
            };

            _mockLoanRepository.Setup(r => r.GetLoanById(loanId)).Returns(loan);

            var result = _service.GetOrCreateFineByLoanId(loanId);

            Assert.That(result, Is.Null);
            _mockFineRepository.Verify(r => r.InsertFine(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<decimal>()), Times.Never);
            _mockFineRepository.Verify(r => r.UpdateFine(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<decimal>()), Times.Never);
        }

        [Test]
        public void GetOrCreateFineByLoanId_DeveInserirMulta_QuandoNaoExisteAinda()
        {
            int loanId = 5;

            var dueDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-10);
            var loan = new TbLoan
            {
                Id = loanId,
                UserCpf = "123",
                Duedate = dueDate,
                Returndate = DateOnly.FromDateTime(DateTime.Now) // 10 dias de atraso
            };

            _mockLoanRepository.Setup(r => r.GetLoanById(loanId)).Returns(loan);

            // Não existe ainda
            _mockFineRepository.Setup(r => r.GetFineByLoanId(loanId)).Returns((TbFine?)null);

            // Depois do insert, o service busca de novo
            _mockFineRepository.SetupSequence(r => r.GetFineByLoanId(loanId))
                .Returns((TbFine?)null)
                .Returns(new TbFine
                {
                    Id = 1,
                    UserCpf = "123",
                    LoanId = loanId,
                    Amount = 3.00m,
                    Dayslate = 10,
                    Ispaid = false,
                    Dailyrate = 0.30m,
                    Paymentdate = null
                });

            var result = _service.GetOrCreateFineByLoanId(loanId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.LoanId, Is.EqualTo(loanId));
            Assert.That(result.DaysLate, Is.EqualTo(10));

            _mockFineRepository.Verify(r => r.InsertFine("123", loanId, It.IsAny<decimal>(), 10, It.IsAny<decimal>()), Times.Once);
        }

        [Test]
        public void GetOrCreateFineByLoanId_DeveAtualizarMulta_QuandoExisteENaoEstaPaga()
        {
            int loanId = 5;

            var dueDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-5);
            var loan = new TbLoan
            {
                Id = loanId,
                UserCpf = "123",
                Duedate = dueDate,
                Returndate = DateOnly.FromDateTime(DateTime.Now) // 5 dias atraso
            };

            _mockLoanRepository.Setup(r => r.GetLoanById(loanId)).Returns(loan);

            // Existe e não está paga
            _mockFineRepository.Setup(r => r.GetFineByLoanId(loanId))
                .Returns(new TbFine
                {
                    Id = 7,
                    UserCpf = "123",
                    LoanId = loanId,
                    Amount = 1.00m,
                    Dayslate = 2,
                    Ispaid = false,
                    Dailyrate = 0.30m
                });

            _mockFineRepository.Setup(r => r.UpdateFine(7, It.IsAny<decimal>(), 5, It.IsAny<decimal>()))
                               .Returns(true);

            // Após atualizar, ele busca de novo
            _mockFineRepository.SetupSequence(r => r.GetFineByLoanId(loanId))
                .Returns(new TbFine { Id = 7, UserCpf = "123", LoanId = loanId, Amount = 1.00m, Dayslate = 2, Ispaid = false, Dailyrate = 0.30m })
                .Returns(new TbFine { Id = 7, UserCpf = "123", LoanId = loanId, Amount = 1.50m, Dayslate = 5, Ispaid = false, Dailyrate = 0.30m });

            var result = _service.GetOrCreateFineByLoanId(loanId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.DaysLate, Is.EqualTo(5));

            _mockFineRepository.Verify(r => r.UpdateFine(7, It.IsAny<decimal>(), 5, It.IsAny<decimal>()), Times.Once);
        }

        [Test]
        public void GetOrCreateFineByLoanId_NaoDeveAtualizar_QuandoMultaJaEstaPaga()
        {
            int loanId = 5;

            var dueDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-5);
            var loan = new TbLoan
            {
                Id = loanId,
                UserCpf = "123",
                Duedate = dueDate,
                Returndate = DateOnly.FromDateTime(DateTime.Now)
            };

            _mockLoanRepository.Setup(r => r.GetLoanById(loanId)).Returns(loan);

            // Existe e está paga
            _mockFineRepository.Setup(r => r.GetFineByLoanId(loanId))
                .Returns(new TbFine
                {
                    Id = 9,
                    UserCpf = "123",
                    LoanId = loanId,
                    Amount = 2.00m,
                    Dayslate = 5,
                    Ispaid = true,
                    Dailyrate = 0.30m
                });

            var result = _service.GetOrCreateFineByLoanId(loanId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsPaid, Is.True);

            _mockFineRepository.Verify(r => r.UpdateFine(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<decimal>()), Times.Never);
        }

        // =====================================================
        // PAY FINE  (seus testes estão ok)
        // =====================================================

        [Test]
        public void PayFine_DeveRetornarError_QuandoIdForInvalido()
        {
            var resultado = _service.PayFine(0);
            Assert.That(resultado, Is.EqualTo("error"));

            _mockFineRepository.Verify(r => r.PayFine(It.IsAny<int>(), It.IsAny<DateOnly>()), Times.Never);
        }

        [Test]
        public void PayFine_DeveRetornarError_QuandoMultaNaoExistir()
        {
            int fineId = 999;

            _mockFineRepository.Setup(r => r.GetFineById(fineId))
                               .Returns((TbFine?)null);

            var resultado = _service.PayFine(fineId);

            Assert.That(resultado, Is.EqualTo("error"));
            _mockFineRepository.Verify(r => r.PayFine(It.IsAny<int>(), It.IsAny<DateOnly>()), Times.Never);
        }

        [Test]
        public void PayFine_DeveRetornarError_QuandoMultaJaEstiverPaga()
        {
            int fineId = 2;

            _mockFineRepository.Setup(r => r.GetFineById(fineId))
                               .Returns(new TbFine { Id = fineId, Ispaid = true });

            var resultado = _service.PayFine(fineId);

            Assert.That(resultado, Is.EqualTo("error"));
            _mockFineRepository.Verify(r => r.PayFine(It.IsAny<int>(), It.IsAny<DateOnly>()), Times.Never);
        }

        [Test]
        public void PayFine_DeveRetornarOk_QuandoMultaExistirENaoEstiverPaga()
        {
            int fineId = 1;

            _mockFineRepository.Setup(r => r.GetFineById(fineId))
                               .Returns(new TbFine { Id = fineId, Ispaid = false });

            _mockFineRepository.Setup(r => r.PayFine(fineId, It.IsAny<DateOnly>()))
                               .Returns(true);

            var resultado = _service.PayFine(fineId);

            Assert.That(resultado, Is.EqualTo(""));
            _mockFineRepository.Verify(r => r.PayFine(fineId, It.IsAny<DateOnly>()), Times.Once);
        }

        [Test]
        public void PayFine_DeveRetornarError_QuandoRepositorioFalhar()
        {
            int fineId = 3;

            _mockFineRepository.Setup(r => r.GetFineById(fineId))
                               .Returns(new TbFine { Id = fineId, Ispaid = false });

            _mockFineRepository.Setup(r => r.PayFine(fineId, It.IsAny<DateOnly>()))
                               .Returns(false);

            var resultado = _service.PayFine(fineId);

            Assert.That(resultado, Is.EqualTo("error"));
            _mockFineRepository.Verify(r => r.PayFine(fineId, It.IsAny<DateOnly>()), Times.Once);
        }

        // =====================================================
        // HAS OPEN FINE
        // =====================================================

        [Test]
        public void HasOpenFineByCpf_DeveRetornarFalse_QuandoCpfVazio()
        {
            var resultado = _service.HasOpenFineByCpf("");

            Assert.That(resultado, Is.False);
            _mockFineRepository.Verify(r => r.HasOpenFineByCpf(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void HasOpenFineByCpf_DeveRetornarTrue_QuandoRepositorioIndicarMulta()
        {
            string cpf = "12345678900";

            _mockFineRepository.Setup(r => r.HasOpenFineByCpf(cpf)).Returns(true);

            var resultado = _service.HasOpenFineByCpf(cpf);

            Assert.That(resultado, Is.True);
            _mockFineRepository.Verify(r => r.HasOpenFineByCpf(cpf), Times.Once);
        }

        [Test]
        public void HasOpenFineByCpf_DeveRetornarFalse_QuandoRepositorioNaoIndicarMulta()
        {
            string cpf = "12345678900";

            _mockFineRepository.Setup(r => r.HasOpenFineByCpf(cpf)).Returns(false);

            var resultado = _service.HasOpenFineByCpf(cpf);

            Assert.That(resultado, Is.False);
            _mockFineRepository.Verify(r => r.HasOpenFineByCpf(cpf), Times.Once);
        }

        // =====================================================
        // GET FINES BY CPF
        // =====================================================

        [Test]
        public void GetFinesByCpf_DeveRetornarListaVazia_QuandoCpfVazio()
        {
            var resultado = _service.GetFinesByCpf("");

            Assert.That(resultado.Count, Is.EqualTo(0));
            _mockFineRepository.Verify(r => r.SelectFinesByCpf(It.IsAny<string>()), Times.Never);
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
                    Paymentdate = null
                }
            };

            _mockFineRepository.Setup(r => r.SelectFinesByCpf(cpf)).Returns(fines);

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
            _mockFineRepository.Verify(r => r.GetFineById(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void GetFineById_DeveRetornarNull_QuandoNaoExistir()
        {
            int id = 10;

            _mockFineRepository.Setup(r => r.GetFineById(id))
                               .Returns((TbFine?)null);

            var resultado = _service.GetFineById(id);

            Assert.That(resultado, Is.Null);
        }

        [Test]
        public void GetFineById_DeveMapearDTO_Corretamente()
        {
            int id = 1;

            _mockFineRepository.Setup(r => r.GetFineById(id))
                               .Returns(new TbFine
                               {
                                   Id = id,
                                   UserCpf = "123",
                                   LoanId = 5,
                                   Amount = 12.30m,
                                   Dayslate = 41,
                                   Ispaid = false,
                                   Dailyrate = 0.30m,
                                   Paymentdate = null
                               });

            var resultado = _service.GetFineById(id);

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado!.Id, Is.EqualTo(id));
            Assert.That(resultado.Amount, Is.EqualTo(12.30m));
        }
    }
}
