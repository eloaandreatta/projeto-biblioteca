using pBiblioteca.Models;

namespace pBiblioteca.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IUserRepository _userRepo;
    private readonly IFineRepository _fineRepo;

    public ReservationService(
        IReservationRepository reservationRepo,
        IBookRepository bookRepo,
        IUserRepository userRepo,
        IFineRepository fineRepo)
    {
        _reservationRepo = reservationRepo ?? throw new ArgumentNullException(nameof(reservationRepo));
        _bookRepo = bookRepo ?? throw new ArgumentNullException(nameof(bookRepo));
        _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        _fineRepo = fineRepo ?? throw new ArgumentNullException(nameof(fineRepo));
    }

    public string CreateReservation(CreateReservationRequest request)
    {
        if (request == null) return "error";

        var user = _userRepo.GetUserById(request.UserCpf);
        if (user == null) return "user_not_found";

        // ✅ Regra 1: multa em aberto bloqueia reserva
        if (_fineRepo.HasOpenFineByCpf(user.Cpf))
            return "user_has_unpaid_fine";

        var book = _bookRepo.GetBookByIsbn(request.BookIsbn);
        if (book == null) return "book_not_found";

        // Regra: reserva só quando indisponível
        if (book.Availablequantity > 0)
            return "book_available_no_need_reservation";

        // Regra: não duplicar reserva ativa
        var existingActive = _reservationRepo.GetActiveReservationForUserAndBook(user.Cpf, request.BookIsbn);
        if (existingActive != null)
            return "reservation_already_exists";

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var reservation = new TbReservation
        {
            UserCpf = user.Cpf,
            Reservationdate = today,
            Notifieddate = DateOnly.MinValue,
            Expirationdate = DateOnly.MinValue,
            Status = true
        };

        _reservationRepo.CreateReservationWithBook(reservation, request.BookIsbn);
        return "ok";
    }

    public string CancelReservation(int reservationId)
    {
        var reservation = _reservationRepo.GetById(reservationId);
        if (reservation == null) return "not_found";

        if (reservation.Status != true)
            return "cannot_cancel";

        reservation.Status = false;
        _reservationRepo.Update(reservation);
        return "ok";
    }

    public int GetUserPosition(string isbn, string cpf)
    {
        var user = _userRepo.GetUserById(cpf);
        if (user == null) return -1;

        var reservation = _reservationRepo.GetActiveReservationForUserAndBook(user.Cpf, isbn);
        if (reservation == null) return -1;

        return _reservationRepo.CountActiveBefore(isbn, reservation.Reservationdate, reservation.Id) + 1;
    }

    public List<ReservationResponseDTO> GetQueue(string isbn)
    {
        var queue = _reservationRepo.GetActiveQueueByBook(isbn);

        var result = new List<ReservationResponseDTO>(queue.Count);

        for (int i = 0; i < queue.Count; i++)
        {
            var r = queue[i];

            result.Add(new ReservationResponseDTO
            {
                ReservationId = r.Id,
                BookIsbn = isbn,
                UserCpf = r.UserCpf,

                Status = r.Status,
                StatusText = r.Status ? "ACTIVE" : "CANCELLED",

                ReservationDate = r.Reservationdate,
                NotifiedDate = r.Notifieddate,
                ExpirationDate = r.Expirationdate,

                Position = i + 1
            });
        }

        return result;
    }

    // ✅ Quando devolve um livro: libera o próximo da fila para retirada (notifica)
    public void NotifyNextIfAny(string isbn, DateOnly notifiedDate, DateOnly expirationDate)
    {
        var next = _reservationRepo.GetNextToNotify(isbn);
        if (next == null) return;

        // Só “notifica” se ainda não foi liberada para retirada
        if (next.Notifieddate == DateOnly.MinValue)
        {
            next.Notifieddate = notifiedDate;
            next.Expirationdate = expirationDate;

            _reservationRepo.Update(next);
        }
    }

    // ✅ Expira automaticamente reservas com prazo vencido (Notifieddate setado e Expirationdate < hoje)
    public int ExpireOverdueReservations(DateOnly today)
    {
        var expired = _reservationRepo.GetReservationsToExpire(today);

        int count = 0;
        foreach (var r in expired)
        {
            r.Status = false;
            _reservationRepo.Update(r);
            count++;
        }

        return count;
    }
}
