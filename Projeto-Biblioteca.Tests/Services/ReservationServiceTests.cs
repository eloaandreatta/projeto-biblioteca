using Moq;
using NUnit.Framework;
using pBiblioteca.Models;
using pBiblioteca.Services;
using Projeto_Biblioteca.Tests.Fixtures.TestData;

namespace Projeto_Biblioteca.Tests.Services;

[TestFixture]
public class ReservationServiceTests
{
    private Mock<IReservationRepository> _reservationRepo = null!;
    private Mock<IBookRepository> _bookRepo = null!;
    private Mock<IUserRepository> _userRepo = null!;
    private ReservationService _service = null!;

    [SetUp]
    public void Setup()
    {
        _reservationRepo = new Mock<IReservationRepository>(MockBehavior.Strict);
        _bookRepo = new Mock<IBookRepository>(MockBehavior.Strict);
        _userRepo = new Mock<IUserRepository>(MockBehavior.Strict);

        _service = new ReservationService(_reservationRepo.Object, _bookRepo.Object, _userRepo.Object);
    }

    [Test]
    public void CreateReservation_QuandoUserNaoExiste_DeveRetornarUserNotFound()
    {
        var req = Factory.CreateReservationRequest();

        _userRepo.Setup(r => r.GetUserById(req.UserCpf)).Returns((TbUser?)null);

        var result = _service.CreateReservation(req);

        Assert.That(result, Is.EqualTo("user_not_found"));
        _bookRepo.Verify(r => r.GetBookByIsbn(It.IsAny<string>()), Times.Never);
        _reservationRepo.Verify(r => r.CreateReservationWithBook(It.IsAny<TbReservation>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void CreateReservation_QuandoLivroNaoExiste_DeveRetornarBookNotFound()
    {
        var req = Factory.CreateReservationRequest();

        _userRepo.Setup(r => r.GetUserById(req.UserCpf)).Returns(Factory.User(cpf: req.UserCpf));
        _bookRepo.Setup(r => r.GetBookByIsbn(req.BookIsbn)).Returns((TbBook?)null);

        var result = _service.CreateReservation(req);

        Assert.That(result, Is.EqualTo("book_not_found"));
        _reservationRepo.Verify(r => r.CreateReservationWithBook(It.IsAny<TbReservation>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void CreateReservation_QuandoLivroDisponivel_DeveRetornarNoNeedReservation()
    {
        var req = Factory.CreateReservationRequest();

        _userRepo.Setup(r => r.GetUserById(req.UserCpf)).Returns(Factory.User(cpf: req.UserCpf));
        _bookRepo.Setup(r => r.GetBookByIsbn(req.BookIsbn)).Returns(Factory.Book(isbn: req.BookIsbn, available: 1));

        var result = _service.CreateReservation(req);

        Assert.That(result, Is.EqualTo("book_available_no_need_reservation"));
        _reservationRepo.Verify(r => r.GetActiveReservationForUserAndBook(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void CreateReservation_QuandoJaExisteReservaAtiva_DeveRetornarAlreadyExists()
    {
        var req = Factory.CreateReservationRequest();
        var user = Factory.User(cpf: req.UserCpf);

        _userRepo.Setup(r => r.GetUserById(req.UserCpf)).Returns(user);
        _bookRepo.Setup(r => r.GetBookByIsbn(req.BookIsbn)).Returns(Factory.Book(isbn: req.BookIsbn, available: 0));

        _reservationRepo.Setup(r => r.GetActiveReservationForUserAndBook(user.Cpf, req.BookIsbn))
            .Returns(Factory.Reservation(id: 9, cpf: user.Cpf, status: true));

        var result = _service.CreateReservation(req);

        Assert.That(result, Is.EqualTo("reservation_already_exists"));
        _reservationRepo.Verify(r => r.CreateReservationWithBook(It.IsAny<TbReservation>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void CreateReservation_QuandoOk_DeveCriarReserva()
    {
        var req = Factory.CreateReservationRequest();
        var user = Factory.User(cpf: req.UserCpf);

        _userRepo.Setup(r => r.GetUserById(req.UserCpf)).Returns(user);
        _bookRepo.Setup(r => r.GetBookByIsbn(req.BookIsbn)).Returns(Factory.Book(isbn: req.BookIsbn, available: 0));

        _reservationRepo.Setup(r => r.GetActiveReservationForUserAndBook(user.Cpf, req.BookIsbn))
            .Returns((TbReservation?)null);

        _reservationRepo.Setup(r => r.CreateReservationWithBook(It.IsAny<TbReservation>(), req.BookIsbn));

        var result = _service.CreateReservation(req);

        Assert.That(result, Is.EqualTo("ok"));
        _reservationRepo.Verify(r => r.CreateReservationWithBook(
            It.Is<TbReservation>(x => x.UserCpf == user.Cpf && x.Status == true),
            req.BookIsbn
        ), Times.Once);
    }

    [Test]
    public void CancelReservation_QuandoNaoExiste_DeveRetornarNotFound()
    {
        _reservationRepo.Setup(r => r.GetById(1)).Returns((TbReservation?)null);

        var result = _service.CancelReservation(1);

        Assert.That(result, Is.EqualTo("not_found"));
        _reservationRepo.Verify(r => r.Update(It.IsAny<TbReservation>()), Times.Never);
    }

    [Test]
    public void CancelReservation_QuandoNaoAtiva_DeveRetornarCannotCancel()
    {
        _reservationRepo.Setup(r => r.GetById(1)).Returns(Factory.Reservation(id: 1, status: false));

        var result = _service.CancelReservation(1);

        Assert.That(result, Is.EqualTo("cannot_cancel"));
        _reservationRepo.Verify(r => r.Update(It.IsAny<TbReservation>()), Times.Never);
    }

    [Test]
    public void CancelReservation_QuandoOk_DeveAtualizarStatusFalse()
    {
        var r = Factory.Reservation(id: 1, status: true);

        _reservationRepo.Setup(x => x.GetById(1)).Returns(r);
        _reservationRepo.Setup(x => x.Update(r));

        var result = _service.CancelReservation(1);

        Assert.That(result, Is.EqualTo("ok"));
        Assert.That(r.Status, Is.False);
        _reservationRepo.Verify(x => x.Update(It.Is<TbReservation>(z => z.Id == 1 && z.Status == false)), Times.Once);
    }

    [Test]
    public void GetUserPosition_QuandoUserNaoExiste_DeveRetornarMenos1()
    {
        _userRepo.Setup(r => r.GetUserById("cpf")).Returns((TbUser?)null);

        var pos = _service.GetUserPosition("isbn", "cpf");

        Assert.That(pos, Is.EqualTo(-1));
    }

    [Test]
    public void GetUserPosition_QuandoReservaNaoExiste_DeveRetornarMenos1()
    {
        var user = Factory.User(cpf: "cpf");

        _userRepo.Setup(r => r.GetUserById("cpf")).Returns(user);
        _reservationRepo.Setup(r => r.GetActiveReservationForUserAndBook(user.Cpf, "isbn")).Returns((TbReservation?)null);

        var pos = _service.GetUserPosition("isbn", "cpf");

        Assert.That(pos, Is.EqualTo(-1));
    }

    [Test]
    public void GetUserPosition_QuandoReservaExiste_DeveRetornarCountMais1()
    {
        var user = Factory.User(cpf: "cpf");
        var res = Factory.Reservation(id: 7, cpf: user.Cpf, status: true, date: DateOnly.FromDateTime(DateTime.Today.AddDays(-2)));

        _userRepo.Setup(r => r.GetUserById("cpf")).Returns(user);
        _reservationRepo.Setup(r => r.GetActiveReservationForUserAndBook(user.Cpf, "isbn")).Returns(res);
        _reservationRepo.Setup(r => r.CountActiveBefore("isbn", res.Reservationdate, res.Id)).Returns(2);

        var pos = _service.GetUserPosition("isbn", "cpf");

        Assert.That(pos, Is.EqualTo(3));
    }

    [Test]
    public void GetQueue_DeveMapearPosicoesEStatusText()
    {
        _reservationRepo.Setup(r => r.GetActiveQueueByBook("isbn"))
            .Returns(new List<TbReservation>
            {
                Factory.Reservation(id: 1, cpf: "a", status: true),
                Factory.Reservation(id: 2, cpf: "b", status: false)
            });

        var list = _service.GetQueue("isbn");

        Assert.That(list.Count, Is.EqualTo(2));
        Assert.That(list[0].Position, Is.EqualTo(1));
        Assert.That(list[0].StatusText, Is.EqualTo("ACTIVE"));
        Assert.That(list[1].Position, Is.EqualTo(2));
        Assert.That(list[1].StatusText, Is.EqualTo("CANCELLED"));
    }
}