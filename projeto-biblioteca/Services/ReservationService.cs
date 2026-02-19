using pBiblioteca.Models;

namespace pBiblioteca.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IUserRepository _userRepo;

    public ReservationService(
        IReservationRepository reservationRepo,
        IBookRepository bookRepo,
        IUserRepository userRepo)
    {
        _reservationRepo = reservationRepo ?? throw new ArgumentNullException(nameof(reservationRepo));
        _bookRepo = bookRepo ?? throw new ArgumentNullException(nameof(bookRepo));
        _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
    }

    public string CreateReservation(CreateReservationRequest request)
    {
        var user = _userRepo.GetUserById(request.Cpf);
        if (user == null) return "user_not_found";

        var book = _bookRepo.GetBookByIsbn(request.Isbn);
        if (book == null) return "book_not_found";

        // Regra: reserva só quando indisponível
        if (book.Availablequantity > 0)
            return "book_available_no_need_reservation";

        // Regra: não duplicar reserva ativa
        var existingActive = _reservationRepo.GetActiveReservationForUserAndBook(user.Cpf, request.Isbn);
        if (existingActive != null)
            return "reservation_already_exists";

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var reservation = new TbReservation
        {
            UserCpf = user.Cpf,
            Reservationdate = today,

            // Como o schema não permite null, usamos MinValue como "ainda não notificado"
            Notifieddate = DateOnly.MinValue,
            Expirationdate = DateOnly.MinValue,

            Status = true
        };

        _reservationRepo.CreateReservationWithBook(reservation, request.Isbn);
        return "ok";
    }

    public string CancelReservation(long reservationId)
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

        var result = new List<ReservationResponseDTO>();
        for (int i = 0; i < queue.Count; i++)
        {
            var r = queue[i];

            DateTime reservationDate = r.Reservationdate.ToDateTime(TimeOnly.MinValue);

            DateTime? pickupDeadline = null;
            if (r.Notifieddate != DateOnly.MinValue && r.Expirationdate != DateOnly.MinValue)
                pickupDeadline = r.Expirationdate.ToDateTime(TimeOnly.MinValue);

            result.Add(new ReservationResponseDTO
            {
                Id = r.Id,
                Isbn = isbn,
                Cpf = r.UserCpf,
                Status = r.Status ? "ACTIVE" : "CANCELLED",
                ReservationDate = reservationDate,
                PickupDeadline = pickupDeadline,
                Position = i + 1
            });
        }

        return result;
    }
}