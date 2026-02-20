using Moq;
using NUnit.Framework;
using pBiblioteca.Models;
using pBiblioteca.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pBiblioteca.Services
{
    [TestFixture]
    public class LoanServiceTests
    {
        private Mock<ILoanRepository> _repoMock = null!;
        private Mock<IFineRepository> _fineRepoMock = null!;
        private Mock<IReservationService> _reservationServiceMock = null!;
        private LoanService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<ILoanRepository>(MockBehavior.Loose);
            _fineRepoMock = new Mock<IFineRepository>(MockBehavior.Loose);
            _reservationServiceMock = new Mock<IReservationService>(MockBehavior.Loose);


            _service = new LoanService(
                _repoMock.Object,
                _fineRepoMock.Object,
                _reservationServiceMock.Object
            );
        }

        // =====================================================
        // GET LOANS
        // =====================================================

        [Test]
        public void GetLoans_DeveMapearListaCorretamente()
        {
            var loans = new List<TbLoan>
            {
                new TbLoan { Id=1, UserCpf="111", BookIsbn="A", Loandate=DateOnly.FromDateTime(DateTime.Now), Duedate=DateOnly.FromDateTime(DateTime.Now).AddDays(14), Returndate=null, Status=true },
                new TbLoan { Id=2, UserCpf="222", BookIsbn="B", Loandate=DateOnly.FromDateTime(DateTime.Now), Duedate=DateOnly.FromDateTime(DateTime.Now).AddDays(14), Returndate=null, Status=false },
            };

            _repoMock.Setup(r => r.SelectLoans()).Returns(loans);

            var result = _service.GetLoans();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[1].UserCpf, Is.EqualTo("222"));

            _repoMock.Verify(r => r.SelectLoans(), Times.Once);
        }

        // =====================================================
        // GET LOAN BY ID
        // =====================================================

        [Test]
        public void GetLoanById_DeveRetornarNull_QuandoIdInvalido()
        {
            var result = _service.GetLoanById(0);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetLoanById_DeveRetornarNull_QuandoNaoExiste()
        {
            _repoMock.Setup(r => r.GetLoanById(10)).Returns((TbLoan?)null);

            var result = _service.GetLoanById(10);

            Assert.That(result, Is.Null);

            _repoMock.Verify(r => r.GetLoanById(10), Times.Once);
        }

        [Test]
        public void GetLoanById_DeveMapearDTO_QuandoExiste()
        {
            var tb = new TbLoan
            {
                Id = 10,
                UserCpf = "123",
                BookIsbn = "ISBN",
                Loandate = DateOnly.FromDateTime(DateTime.Now),
                Duedate = DateOnly.FromDateTime(DateTime.Now).AddDays(14),
                Returndate = null,
                Status = true
            };

            _repoMock.Setup(r => r.GetLoanById(10)).Returns(tb);

            var result = _service.GetLoanById(10);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(10));
            Assert.That(result.BookIsbn, Is.EqualTo("ISBN"));

            _repoMock.Verify(r => r.GetLoanById(10), Times.Once);
        }

        // =====================================================
        // GET LOANS BY USER
        // =====================================================

        [Test]
        public void GetLoansByUser_DeveMapearLista()
        {
            var tbLoans = new List<TbLoan>
            {
                new TbLoan { Id=1, UserCpf="123", BookIsbn="A", Loandate=DateOnly.FromDateTime(DateTime.Now), Duedate=DateOnly.FromDateTime(DateTime.Now).AddDays(14), Status=true }
            };

            _repoMock.Setup(r => r.GetLoansByUserCpf("123")).Returns(tbLoans);

            var result = _service.GetLoansByUser("123");

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].UserCpf, Is.EqualTo("123"));

            _repoMock.Verify(r => r.GetLoansByUserCpf("123"), Times.Once);
           
        }

        // =====================================================
        // CREATE LOAN
        // =====================================================

        [Test]
        public void CreateLoan_DeveRetornarError_QuandoRequestNull()
        {
            var result = _service.CreateLoan(null!);
            Assert.That(result, Is.EqualTo("error"));

        
        }

        [Test]
        public void CreateLoan_DeveRetornarError_QuandoUsuarioNaoExiste()
        {
            var req = new CreateLoanRequest { UserCpf = "123", BookIsbn = "ISBN" };

            _repoMock.Setup(r => r.GetUserByCpf("123")).Returns((TbUser?)null);

            var result = _service.CreateLoan(req);

            Assert.That(result, Is.EqualTo("error"));

            _repoMock.Verify(r => r.GetUserByCpf("123"), Times.Once);

        }

        [Test]
        public void CreateLoan_DeveRetornarError_QuandoUsuarioInativo()
        {
            var req = new CreateLoanRequest { UserCpf = "123", BookIsbn = "ISBN" };

            _repoMock.Setup(r => r.GetUserByCpf("123")).Returns(new TbUser { Cpf = "123", Active = false });

            var result = _service.CreateLoan(req);

            Assert.That(result, Is.EqualTo("error"));

            _repoMock.Verify(r => r.GetUserByCpf("123"), Times.Once);

        }

        [Test]
        public void CreateLoan_DeveRetornarError_QuandoUsuarioJaTemEmprestimoAtivo()
        {
            var req = new CreateLoanRequest { UserCpf = "123", BookIsbn = "ISBN" };

            _repoMock.Setup(r => r.GetUserByCpf("123")).Returns(new TbUser { Cpf = "123", Active = true });
            _repoMock.Setup(r => r.UserHasActiveLoan("123")).Returns(true);

            var result = _service.CreateLoan(req);

            Assert.That(result, Is.EqualTo("error"));

            _repoMock.Verify(r => r.GetUserByCpf("123"), Times.Once);
            _repoMock.Verify(r => r.UserHasActiveLoan("123"), Times.Once);

        }

        [Test]
        public void CreateLoan_DeveRetornarError_QuandoUsuarioTemMultaEmAberto()
        {
            var req = new CreateLoanRequest { UserCpf = "123", BookIsbn = "ISBN" };

            _repoMock.Setup(r => r.GetUserByCpf("123")).Returns(new TbUser { Cpf = "123", Active = true });
            _repoMock.Setup(r => r.UserHasActiveLoan("123")).Returns(false);
            _fineRepoMock.Setup(f => f.HasOpenFineByCpf("123")).Returns(true);

            var result = _service.CreateLoan(req);

            Assert.That(result, Is.EqualTo("error"));

            _repoMock.Verify(r => r.GetUserByCpf("123"), Times.Once);
            _repoMock.Verify(r => r.UserHasActiveLoan("123"), Times.Once);
            _fineRepoMock.Verify(f => f.HasOpenFineByCpf("123"), Times.Once);

        }

        [Test]
        public void CreateLoan_DeveRetornarError_QuandoLivroNaoExiste()
        {
            var req = new CreateLoanRequest { UserCpf = "123", BookIsbn = "ISBN" };

            _repoMock.Setup(r => r.GetUserByCpf("123")).Returns(new TbUser { Cpf = "123", Active = true });
            _repoMock.Setup(r => r.UserHasActiveLoan("123")).Returns(false);
            _fineRepoMock.Setup(f => f.HasOpenFineByCpf("123")).Returns(false);
            _repoMock.Setup(r => r.GetBookByIsbn("ISBN")).Returns((TbBook?)null);

            var result = _service.CreateLoan(req);

            Assert.That(result, Is.EqualTo("error"));

            _repoMock.Verify(r => r.GetUserByCpf("123"), Times.Once);
            _repoMock.Verify(r => r.UserHasActiveLoan("123"), Times.Once);
            _fineRepoMock.Verify(f => f.HasOpenFineByCpf("123"), Times.Once);
            _repoMock.Verify(r => r.GetBookByIsbn("ISBN"), Times.Once);

        }

        [Test]
        public void CreateLoan_DeveEntrarNaFila_QuandoLivroSemEstoque_RetornaQueued()
        {
            var req = new CreateLoanRequest { UserCpf = "123", BookIsbn = "ISBN" };

            _repoMock.Setup(r => r.GetUserByCpf("123")).Returns(new TbUser { Cpf = "123", Active = true });
            _repoMock.Setup(r => r.UserHasActiveLoan("123")).Returns(false);
            _fineRepoMock.Setup(f => f.HasOpenFineByCpf("123")).Returns(false);

            _repoMock.Setup(r => r.GetBookByIsbn("ISBN")).Returns(new TbBook { Isbn = "ISBN", Availablequantity = 0 });

            _reservationServiceMock.Setup(s => s.CreateReservation(It.Is<CreateReservationRequest>(x =>
                x.UserCpf == "123" && x.BookIsbn == "ISBN"
            ))).Returns("ok");

            var result = _service.CreateLoan(req);

            Assert.That(result, Is.EqualTo("queued_for_reservation"));

            _repoMock.Verify(r => r.InsertLoan(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>()), Times.Never);
            _repoMock.Verify(r => r.Save(), Times.Never);

            _repoMock.Verify(r => r.GetUserByCpf("123"), Times.Once);
            _repoMock.Verify(r => r.UserHasActiveLoan("123"), Times.Once);
            _fineRepoMock.Verify(f => f.HasOpenFineByCpf("123"), Times.Once);
            _repoMock.Verify(r => r.GetBookByIsbn("ISBN"), Times.Once);
            _reservationServiceMock.Verify(s => s.CreateReservation(It.IsAny<CreateReservationRequest>()), Times.Once);

        }

        [Test]
        public void CreateLoan_DeveRetornarUserHasUnpaidFine_QuandoReservationServiceRetornarUserHasUnpaidFine()
        {
            var req = new CreateLoanRequest { UserCpf = "123", BookIsbn = "ISBN" };

            _repoMock.Setup(r => r.GetUserByCpf("123")).Returns(new TbUser { Cpf = "123", Active = true });
            _repoMock.Setup(r => r.UserHasActiveLoan("123")).Returns(false);
            _fineRepoMock.Setup(f => f.HasOpenFineByCpf("123")).Returns(false);
            _repoMock.Setup(r => r.GetBookByIsbn("ISBN")).Returns(new TbBook { Isbn = "ISBN", Availablequantity = 0 });

            _reservationServiceMock.Setup(s => s.CreateReservation(It.IsAny<CreateReservationRequest>()))
                                  .Returns("user_has_unpaid_fine");

            var result = _service.CreateLoan(req);

            Assert.That(result, Is.EqualTo("user_has_unpaid_fine"));

            _repoMock.Verify(r => r.Save(), Times.Never);
            _repoMock.Verify(r => r.InsertLoan(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>()), Times.Never);

        }

        [Test]
        public void CreateLoan_DeveCriarEmprestimo_QuandoDadosValidos()
        {
            var req = new CreateLoanRequest { UserCpf = "123", BookIsbn = "ISBN" };

            var book = new TbBook { Isbn = "ISBN", Availablequantity = 2 };

            _repoMock.Setup(r => r.GetUserByCpf("123")).Returns(new TbUser { Cpf = "123", Active = true });
            _repoMock.Setup(r => r.UserHasActiveLoan("123")).Returns(false);
            _fineRepoMock.Setup(f => f.HasOpenFineByCpf("123")).Returns(false);

            _repoMock.Setup(r => r.GetBookByIsbn("ISBN")).Returns(book);

            _repoMock.Setup(r => r.InsertLoan("123", "ISBN", It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
                     .Returns(true);

            _repoMock.Setup(r => r.Save());

            var result = _service.CreateLoan(req);

            Assert.That(result, Is.EqualTo(""));

            Assert.That(book.Availablequantity, Is.EqualTo(1));

            _repoMock.Verify(r => r.InsertLoan("123", "ISBN", It.IsAny<DateOnly>(), It.IsAny<DateOnly>()), Times.Once);
            _repoMock.Verify(r => r.Save(), Times.Once);

        }

        [Test]
        public void CreateLoan_DeveRetornarError_QuandoRepositorioFalharInsertLoan()
        {
            var req = new CreateLoanRequest { UserCpf = "123", BookIsbn = "ISBN" };
            var book = new TbBook { Isbn = "ISBN", Availablequantity = 1 };

            _repoMock.Setup(r => r.GetUserByCpf("123")).Returns(new TbUser { Cpf = "123", Active = true });
            _repoMock.Setup(r => r.UserHasActiveLoan("123")).Returns(false);
            _fineRepoMock.Setup(f => f.HasOpenFineByCpf("123")).Returns(false);
            _repoMock.Setup(r => r.GetBookByIsbn("ISBN")).Returns(book);

            _repoMock.Setup(r => r.InsertLoan("123", "ISBN", It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
                     .Returns(false);

            var result = _service.CreateLoan(req);

            Assert.That(result, Is.EqualTo("error"));
            Assert.That(book.Availablequantity, Is.EqualTo(1));

            _repoMock.Verify(r => r.InsertLoan("123", "ISBN", It.IsAny<DateOnly>(), It.IsAny<DateOnly>()), Times.Once);
            _repoMock.Verify(r => r.Save(), Times.Never);

        }

        // =====================================================
        // RETURN LOAN
        // =====================================================

        [Test]
        public void ReturnLoan_DeveRetornarError_QuandoIdInvalido()
        {
            var result = _service.ReturnLoan(0);
            Assert.That(result, Is.EqualTo("error"));

        }

        [Test]
        public void ReturnLoan_DeveRetornarError_QuandoLoanNaoExiste()
        {
            _repoMock.Setup(r => r.GetLoanById(10)).Returns((TbLoan?)null);

            var result = _service.ReturnLoan(10);

            Assert.That(result, Is.EqualTo("error"));

            _repoMock.Verify(r => r.GetLoanById(10), Times.Once);

        }

        [Test]
        public void ReturnLoan_DeveRetornarError_QuandoLoanJaFechado()
        {
            _repoMock.Setup(r => r.GetLoanById(10)).Returns(new TbLoan { Id = 10, Status = false });

            var result = _service.ReturnLoan(10);

            Assert.That(result, Is.EqualTo("error"));

            _repoMock.Verify(r => r.GetLoanById(10), Times.Once);

        }

        [Test]
        public void ReturnLoan_DeveRetornarError_QuandoLivroNaoExiste()
        {
            var loan = new TbLoan
            {
                Id = 10,
                UserCpf = "123",
                BookIsbn = "ISBN",
                Loandate = DateOnly.FromDateTime(DateTime.Now).AddDays(-2),
                Duedate = DateOnly.FromDateTime(DateTime.Now).AddDays(10),
                Status = true
            };

            _repoMock.Setup(r => r.GetLoanById(10)).Returns(loan);
            _repoMock.Setup(r => r.GetBookByIsbn("ISBN")).Returns((TbBook?)null);

            var result = _service.ReturnLoan(10);

            Assert.That(result, Is.EqualTo("error"));

            _repoMock.Verify(r => r.GetLoanById(10), Times.Once);
            _repoMock.Verify(r => r.GetBookByIsbn("ISBN"), Times.Once);

        }

        [Test]
        public void ReturnLoan_SemAtraso_NaoGeraMulta_E_NotificaSeEstoquePositivo()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var loan = new TbLoan
            {
                Id = 10,
                UserCpf = "123",
                BookIsbn = "ISBN",
                Duedate = today.AddDays(5),
                Status = true
            };

            var book = new TbBook
            {
                Isbn = "ISBN",
                Availablequantity = 0
            };

            _repoMock.Setup(r => r.GetLoanById(10)).Returns(loan);
            _repoMock.Setup(r => r.GetBookByIsbn("ISBN")).Returns(book);

            _repoMock.Setup(r => r.UpdateLoan(10, It.IsAny<DateOnly?>(), false)).Returns(true);

            // como incrementa estoque, ficará 1 -> notifica
            _reservationServiceMock.Setup(s => s.NotifyNextIfAny(
                "ISBN",
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>()
            ));

            _repoMock.Setup(r => r.Save());

            var result = _service.ReturnLoan(10);

            Assert.That(result, Is.EqualTo(""));
            Assert.That(book.Availablequantity, Is.EqualTo(1));

            _repoMock.Verify(r => r.AddFine(It.IsAny<TbFine>()), Times.Never);
            _reservationServiceMock.Verify(s => s.NotifyNextIfAny("ISBN", It.IsAny<DateOnly>(), It.IsAny<DateOnly>()), Times.Once);
            _repoMock.Verify(r => r.Save(), Times.Once);

        }

        [Test]
        public void ReturnLoan_ComAtraso_GeraMulta()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var loan = new TbLoan
            {
                Id = 10,
                UserCpf = "123",
                BookIsbn = "ISBN",
                Duedate = today.AddDays(-3), // atraso 3
                Status = true
            };

            var book = new TbBook
            {
                Isbn = "ISBN",
                Availablequantity = 0
            };

            _repoMock.Setup(r => r.GetLoanById(10)).Returns(loan);
            _repoMock.Setup(r => r.GetBookByIsbn("ISBN")).Returns(book);

            _repoMock.Setup(r => r.UpdateLoan(10, It.IsAny<DateOnly?>(), false)).Returns(true);

            _reservationServiceMock.Setup(s => s.NotifyNextIfAny("ISBN", It.IsAny<DateOnly>(), It.IsAny<DateOnly>()));

            _repoMock.Setup(r => r.AddFine(It.Is<TbFine>(f =>
                f.UserCpf == "123" &&
                f.LoanId == 10 &&
                f.Dayslate == 3 &&
                f.Amount > 0 &&
                f.Ispaid == false
            )));

            _repoMock.Setup(r => r.Save());

            var result = _service.ReturnLoan(10);

            Assert.That(result, Is.EqualTo(""));
            Assert.That(book.Availablequantity, Is.EqualTo(1));

            _repoMock.Verify(r => r.AddFine(It.IsAny<TbFine>()), Times.Once);
            _repoMock.Verify(r => r.Save(), Times.Once);

        }

        [Test]
        public void ReturnLoan_NaoNotifica_QuandoUpdateLoanFalhar()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var loan = new TbLoan
            {
                Id = 10,
                UserCpf = "123",
                BookIsbn = "ISBN",
                Duedate = today.AddDays(5),
                Status = true
            };

            var book = new TbBook { Isbn = "ISBN", Availablequantity = 0 };

            _repoMock.Setup(r => r.GetLoanById(10)).Returns(loan);
            _repoMock.Setup(r => r.GetBookByIsbn("ISBN")).Returns(book);

            _repoMock.Setup(r => r.UpdateLoan(10, It.IsAny<DateOnly?>(), false)).Returns(false);

            var result = _service.ReturnLoan(10);

            Assert.That(result, Is.EqualTo("error"));

            _reservationServiceMock.Verify(s => s.NotifyNextIfAny(It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>()), Times.Never);
            _repoMock.Verify(r => r.Save(), Times.Never);

        }

        // =====================================================
        // RENEW LOAN
        // =====================================================

        [Test]
        public void RenewLoan_DeveRetornarError_QuandoLoanNaoExiste()
        {
            _repoMock.Setup(r => r.GetLoanById(10)).Returns((TbLoan?)null);

            var result = _service.RenewLoan(10);

            Assert.That(result, Is.EqualTo("error"));

            _repoMock.Verify(r => r.GetLoanById(10), Times.Once);

        }

        [Test]
        public void RenewLoan_DeveRetornarError_QuandoLoanFechado()
        {
            _repoMock.Setup(r => r.GetLoanById(10)).Returns(new TbLoan { Id = 10, Status = false, UserCpf = "123" });

            var result = _service.RenewLoan(10);

            Assert.That(result, Is.EqualTo("error"));

            _repoMock.Verify(r => r.GetLoanById(10), Times.Once);

        }

        [Test]
        public void RenewLoan_DeveRetornarError_QuandoUsuarioTemMulta()
        {
            _repoMock.Setup(r => r.GetLoanById(10)).Returns(new TbLoan { Id = 10, Status = true, UserCpf = "123" });
            _fineRepoMock.Setup(f => f.HasOpenFineByCpf("123")).Returns(true);

            var result = _service.RenewLoan(10);

            Assert.That(result, Is.EqualTo("error"));

            _repoMock.Verify(r => r.GetLoanById(10), Times.Once);
            _fineRepoMock.Verify(f => f.HasOpenFineByCpf("123"), Times.Once);

        }

        [Test]
        public void RenewLoan_DeveRetornarError_QuandoEmprestimoAtrasado()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var loan = new TbLoan
            {
                Id = 10,
                Status = true,
                UserCpf = "123",
                Loandate = today.AddDays(-20),
                Duedate = today.AddDays(-1) // atrasado
            };

            _repoMock.Setup(r => r.GetLoanById(10)).Returns(loan);
            _fineRepoMock.Setup(f => f.HasOpenFineByCpf("123")).Returns(false);

            var result = _service.RenewLoan(10);

            Assert.That(result, Is.EqualTo("error"));

        }

        [Test]
        public void RenewLoan_DeveRetornarError_QuandoJaAtingiuMaximo28Dias()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var loan = new TbLoan
            {
                Id = 10,
                Status = true,
                UserCpf = "123",
                Loandate = today.AddDays(-30),
                Duedate = today.AddDays(-2).AddDays(28) // igual ou maior que loandate+28 (forçando cenário)
            };

            // Para garantir: maxDueDate = loandate + 28.
            // Vamos setar dueDate exatamente maxDueDate.
            loan.Duedate = loan.Loandate.AddDays(28);

            _repoMock.Setup(r => r.GetLoanById(10)).Returns(loan);
            _fineRepoMock.Setup(f => f.HasOpenFineByCpf("123")).Returns(false);

            var result = _service.RenewLoan(10);

            Assert.That(result, Is.EqualTo("error"));

        }

        [Test]
        public void RenewLoan_DeveRenovar_ComSucesso()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var loan = new TbLoan
            {
                Id = 10,
                Status = true,
                UserCpf = "123",
                Loandate = today.AddDays(-10),
                Duedate = today.AddDays(4) // ainda ok
            };

            _repoMock.Setup(r => r.GetLoanById(10)).Returns(loan);
            _fineRepoMock.Setup(f => f.HasOpenFineByCpf("123")).Returns(false);

            _repoMock.Setup(r => r.RenewLoan(10, It.IsAny<DateOnly>())).Returns(true);
            _repoMock.Setup(r => r.Save());

            var result = _service.RenewLoan(10);

            Assert.That(result, Is.EqualTo("ok"));

            _repoMock.Verify(r => r.RenewLoan(10, It.Is<DateOnly>(d => d == loan.Duedate.AddDays(14))), Times.Once);
            _repoMock.Verify(r => r.Save(), Times.Once);

        }

        [Test]
        public void RenewLoan_DeveRetornarError_QuandoRepositorioFalhar()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var loan = new TbLoan
            {
                Id = 10,
                Status = true,
                UserCpf = "123",
                Loandate = today.AddDays(-10),
                Duedate = today.AddDays(4)
            };

            _repoMock.Setup(r => r.GetLoanById(10)).Returns(loan);
            _fineRepoMock.Setup(f => f.HasOpenFineByCpf("123")).Returns(false);

            _repoMock.Setup(r => r.RenewLoan(10, It.IsAny<DateOnly>())).Returns(false);

            var result = _service.RenewLoan(10);

            Assert.That(result, Is.EqualTo("error"));

            _repoMock.Verify(r => r.Save(), Times.Never);

        }
    }
}
