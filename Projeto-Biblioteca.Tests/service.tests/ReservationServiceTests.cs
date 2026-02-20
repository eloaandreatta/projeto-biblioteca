using Moq;
using NUnit.Framework;
using pBiblioteca.Models;
using pBiblioteca.Services;
using System.Collections.Generic;

namespace Projeto_Biblioteca.Tests
{
    [TestFixture]
    public class ReservationServiceTests
    {
        private Mock<IReservationRepository> _reservationRepoMock;
        private Mock<IBookRepository> _bookRepoMock;
        private Mock<IUserRepository> _userRepoMock;
        private Mock<IFineRepository> _fineRepoMock;

        private ReservationService _service;

        [SetUp]
        public void Setup()
        {
            _reservationRepoMock = new Mock<IReservationRepository>();
            _bookRepoMock = new Mock<IBookRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _fineRepoMock = new Mock<IFineRepository>();

            _service = new ReservationService(
                _reservationRepoMock.Object,
                _bookRepoMock.Object,
                _userRepoMock.Object,
                _fineRepoMock.Object
            );
        }

        // =====================================================
        // CREATE RESERVATION
        // =====================================================

        [Test]
        public void CreateReservation_DeveRetornarUserNotFound_QuandoUsuarioNaoExiste()
        {
            var request = new CreateReservationRequest { UserCpf = "123", BookIsbn = "isbn" };

            _userRepoMock.Setup(r => r.GetUserById(request.UserCpf))
                        .Returns((TbUser?)null);

            var result = _service.CreateReservation(request);

            Assert.That(result, Is.EqualTo("user_not_found"));

            _fineRepoMock.Verify(f => f.HasOpenFineByCpf(It.IsAny<string>()), Times.Never);
            _bookRepoMock.Verify(b => b.GetBookByIsbn(It.IsAny<string>()), Times.Never);
            _reservationRepoMock.Verify(r => r.CreateReservationWithBook(It.IsAny<TbReservation>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void CreateReservation_DeveRetornarUserHasUnpaidFine_QuandoUsuarioTemMultaEmAberto()
        {
            var request = new CreateReservationRequest { UserCpf = "123", BookIsbn = "isbn" };

            _userRepoMock.Setup(r => r.GetUserById("123"))
                         .Returns(new TbUser { Cpf = "123" });

            _fineRepoMock.Setup(f => f.HasOpenFineByCpf("123"))
                         .Returns(true);

            var result = _service.CreateReservation(request);

            Assert.That(result, Is.EqualTo("user_has_unpaid_fine"));

            _bookRepoMock.Verify(b => b.GetBookByIsbn(It.IsAny<string>()), Times.Never);
            _reservationRepoMock.Verify(r => r.CreateReservationWithBook(It.IsAny<TbReservation>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void CreateReservation_DeveRetornarBookNotFound_QuandoLivroNaoExiste()
        {
            var request = new CreateReservationRequest { UserCpf = "123", BookIsbn = "isbn" };

            _userRepoMock.Setup(r => r.GetUserById(request.UserCpf))
                        .Returns(new TbUser { Cpf = "123" });

            _fineRepoMock.Setup(f => f.HasOpenFineByCpf("123")).Returns(false);

            _bookRepoMock.Setup(b => b.GetBookByIsbn(request.BookIsbn))
                        .Returns((TbBook?)null);

            var result = _service.CreateReservation(request);

            Assert.That(result, Is.EqualTo("book_not_found"));

            _reservationRepoMock.Verify(r => r.CreateReservationWithBook(It.IsAny<TbReservation>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void CreateReservation_DeveRetornarBookAvailableNoNeedReservation_QuandoLivroTemEstoque()
        {
            var request = new CreateReservationRequest { UserCpf = "123", BookIsbn = "isbn" };

            _userRepoMock.Setup(r => r.GetUserById(request.UserCpf))
                        .Returns(new TbUser { Cpf = "123" });

            _fineRepoMock.Setup(f => f.HasOpenFineByCpf("123")).Returns(false);

            _bookRepoMock.Setup(b => b.GetBookByIsbn(request.BookIsbn))
                        .Returns(new TbBook { Isbn = "isbn", Availablequantity = 2 });

            var result = _service.CreateReservation(request);

            Assert.That(result, Is.EqualTo("book_available_no_need_reservation"));

            _reservationRepoMock.Verify(r => r.GetActiveReservationForUserAndBook(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _reservationRepoMock.Verify(r => r.CreateReservationWithBook(It.IsAny<TbReservation>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void CreateReservation_DeveRetornarReservationAlreadyExists_QuandoJaExisteReservaAtiva()
        {
            var request = new CreateReservationRequest { UserCpf = "123", BookIsbn = "isbn" };

            _userRepoMock.Setup(r => r.GetUserById(request.UserCpf))
                        .Returns(new TbUser { Cpf = "123" });

            _fineRepoMock.Setup(f => f.HasOpenFineByCpf("123")).Returns(false);

            _bookRepoMock.Setup(b => b.GetBookByIsbn(request.BookIsbn))
                        .Returns(new TbBook { Isbn = "isbn", Availablequantity = 0 });

            _reservationRepoMock.Setup(r => r.GetActiveReservationForUserAndBook("123", "isbn"))
                                .Returns(new TbReservation { Id = 99, UserCpf = "123", Status = true });

            var result = _service.CreateReservation(request);

            Assert.That(result, Is.EqualTo("reservation_already_exists"));

            _reservationRepoMock.Verify(r => r.CreateReservationWithBook(It.IsAny<TbReservation>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void CreateReservation_DeveCriarReservaERetornarOk_QuandoValida()
        {
            var request = new CreateReservationRequest { UserCpf = "123", BookIsbn = "isbn" };

            _userRepoMock.Setup(r => r.GetUserById(request.UserCpf))
                        .Returns(new TbUser { Cpf = "123" });

            _fineRepoMock.Setup(f => f.HasOpenFineByCpf("123")).Returns(false);

            _bookRepoMock.Setup(b => b.GetBookByIsbn(request.BookIsbn))
                        .Returns(new TbBook { Isbn = "isbn", Availablequantity = 0 });

            _reservationRepoMock.Setup(r => r.GetActiveReservationForUserAndBook("123", "isbn"))
                                .Returns((TbReservation?)null);

            var result = _service.CreateReservation(request);

            Assert.That(result, Is.EqualTo("ok"));

            _reservationRepoMock.Verify(r =>
                r.CreateReservationWithBook(
                    It.Is<TbReservation>(res =>
                        res.UserCpf == "123" &&
                        res.Status == true &&
                        res.Notifieddate == DateOnly.MinValue &&
                        res.Expirationdate == DateOnly.MinValue
                    ),
                    "isbn"
                ),
                Times.Once
            );
        }

        // =====================================================
        // CANCEL RESERVATION
        // =====================================================

        [Test]
        public void CancelReservation_DeveRetornarNotFound_QuandoNaoExistir()
        {
            _reservationRepoMock.Setup(r => r.GetById(10)).Returns((TbReservation?)null);

            var result = _service.CancelReservation(10);

            Assert.That(result, Is.EqualTo("not_found"));
            _reservationRepoMock.Verify(r => r.Update(It.IsAny<TbReservation>()), Times.Never);
        }

        [Test]
        public void CancelReservation_DeveRetornarCannotCancel_QuandoStatusFalse()
        {
            _reservationRepoMock.Setup(r => r.GetById(10))
                                .Returns(new TbReservation { Id = 10, Status = false });

            var result = _service.CancelReservation(10);

            Assert.That(result, Is.EqualTo("cannot_cancel"));
            _reservationRepoMock.Verify(r => r.Update(It.IsAny<TbReservation>()), Times.Never);
        }

        [Test]
        public void CancelReservation_DeveCancelarERetornarOk()
        {
            var reservation = new TbReservation { Id = 10, Status = true };

            _reservationRepoMock.Setup(r => r.GetById(10)).Returns(reservation);

            var result = _service.CancelReservation(10);

            Assert.That(result, Is.EqualTo("ok"));
            _reservationRepoMock.Verify(r => r.Update(It.Is<TbReservation>(x => x.Id == 10 && x.Status == false)), Times.Once);
        }

        // =====================================================
        // GET USER POSITION
        // =====================================================

        [Test]
        public void GetUserPosition_DeveRetornarMenos1_QuandoUsuarioNaoExiste()
        {
            _userRepoMock.Setup(r => r.GetUserById("123")).Returns((TbUser?)null);

            var result = _service.GetUserPosition("isbn", "123");

            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void GetUserPosition_DeveRetornarMenos1_QuandoReservaNaoExiste()
        {
            _userRepoMock.Setup(r => r.GetUserById("123")).Returns(new TbUser { Cpf = "123" });

            _reservationRepoMock.Setup(r => r.GetActiveReservationForUserAndBook("123", "isbn"))
                                .Returns((TbReservation?)null);

            var result = _service.GetUserPosition("isbn", "123");

            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void GetUserPosition_DeveRetornarPosicao_Corretamente()
        {
            _userRepoMock.Setup(r => r.GetUserById("123")).Returns(new TbUser { Cpf = "123" });

            var reservation = new TbReservation
            {
                Id = 20,
                UserCpf = "123",
                Status = true,
                Reservationdate = DateOnly.FromDateTime(System.DateTime.Now).AddDays(-2)
            };

            _reservationRepoMock.Setup(r => r.GetActiveReservationForUserAndBook("123", "isbn"))
                                .Returns(reservation);

            _reservationRepoMock.Setup(r => r.CountActiveBefore("isbn", reservation.Reservationdate, reservation.Id))
                                .Returns(3);

            var result = _service.GetUserPosition("isbn", "123");

            Assert.That(result, Is.EqualTo(4));
        }

        // =====================================================
        // GET QUEUE
        // =====================================================

        [Test]
        public void GetQueue_DeveRetornarListaMapeada_ComPosicao()
        {
            var isbn = "isbn";

            var queue = new List<TbReservation>
            {
                new TbReservation
                {
                    Id = 1, UserCpf = "111", Status = true,
                    Reservationdate = DateOnly.FromDateTime(System.DateTime.Now).AddDays(-3),
                    Notifieddate = DateOnly.MinValue, Expirationdate = DateOnly.MinValue
                },
                new TbReservation
                {
                    Id = 2, UserCpf = "222", Status = true,
                    Reservationdate = DateOnly.FromDateTime(System.DateTime.Now).AddDays(-2),
                    Notifieddate = DateOnly.MinValue, Expirationdate = DateOnly.MinValue
                }
            };

            _reservationRepoMock.Setup(r => r.GetActiveQueueByBook(isbn)).Returns(queue);

            var result = _service.GetQueue(isbn);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Position, Is.EqualTo(1));
            Assert.That(result[1].Position, Is.EqualTo(2));
            Assert.That(result[0].UserCpf, Is.EqualTo("111"));
        }

        // =====================================================
        // NOTIFY NEXT IF ANY
        // =====================================================

        [Test]
        public void NotifyNextIfAny_NaoDeveFazerNada_QuandoNaoHaProximo()
        {
            _reservationRepoMock.Setup(r => r.GetNextToNotify("isbn"))
                                .Returns((TbReservation?)null);

            _service.NotifyNextIfAny(
                "isbn",
                DateOnly.FromDateTime(System.DateTime.Now),
                DateOnly.FromDateTime(System.DateTime.Now).AddDays(3));

            _reservationRepoMock.Verify(r => r.Update(It.IsAny<TbReservation>()), Times.Never);
        }

        [Test]
        public void NotifyNextIfAny_DeveAtualizarNotifiedEExpiration_QuandoHaProximo()
        {
            var today = DateOnly.FromDateTime(System.DateTime.Now);
            var expires = today.AddDays(3);

            var next = new TbReservation
            {
                Id = 10,
                UserCpf = "123",
                Status = true,
                Notifieddate = DateOnly.MinValue,
                Expirationdate = DateOnly.MinValue
            };

            _reservationRepoMock.Setup(r => r.GetNextToNotify("isbn"))
                                .Returns(next);

            _service.NotifyNextIfAny("isbn", today, expires);

            _reservationRepoMock.Verify(r => r.Update(It.Is<TbReservation>(x =>
                x.Id == 10 &&
                x.Notifieddate == today &&
                x.Expirationdate == expires
            )), Times.Once);
        }

        // =====================================================
        // EXPIRE OVERDUE RESERVATIONS (retorna int)
        // =====================================================

        [Test]
        public void ExpireOverdueReservations_DeveRetornar0_QuandoNaoHaVencidas()
        {
            var today = DateOnly.FromDateTime(System.DateTime.Now);

            _reservationRepoMock.Setup(r => r.GetReservationsToExpire(today))
                                .Returns(new List<TbReservation>());

            var expiredCount = _service.ExpireOverdueReservations(today);

            Assert.That(expiredCount, Is.EqualTo(0));
            _reservationRepoMock.Verify(r => r.Update(It.IsAny<TbReservation>()), Times.Never);
        }

        [Test]
        public void ExpireOverdueReservations_DeveCancelarVencidas_E_RetornarQuantidade()
        {
            var today = DateOnly.FromDateTime(System.DateTime.Now);

            var expired1 = new TbReservation
            {
                Id = 7,
                UserCpf = "111",
                Status = true,
                Notifieddate = today.AddDays(-4),
                Expirationdate = today.AddDays(-1)
            };

            var expired2 = new TbReservation
            {
                Id = 8,
                UserCpf = "222",
                Status = true,
                Notifieddate = today.AddDays(-5),
                Expirationdate = today.AddDays(-2)
            };

            _reservationRepoMock.Setup(r => r.GetReservationsToExpire(today))
                                .Returns(new List<TbReservation> { expired1, expired2 });

            var expiredCount = _service.ExpireOverdueReservations(today);

            Assert.That(expiredCount, Is.EqualTo(2));

            _reservationRepoMock.Verify(r => r.Update(It.Is<TbReservation>(x => x.Id == 7 && x.Status == false)), Times.Once);
            _reservationRepoMock.Verify(r => r.Update(It.Is<TbReservation>(x => x.Id == 8 && x.Status == false)), Times.Once);
        }
    }
}
