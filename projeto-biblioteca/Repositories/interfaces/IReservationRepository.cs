using pBiblioteca.Models;

public interface IReservationRepository
{
    TbReservation? GetById(int id);
    TbReservation? GetActiveReservationForUserAndBook(string cpf, string isbn);
    List<TbReservation> GetActiveQueueByBook(string isbn);
    int CountActiveBefore(string isbn, DateOnly reservationDate, int reservationId);
    void CreateReservationWithBook(TbReservation reservation, string isbn);
    void Update(TbReservation reservation);

    // NOVOS
    TbReservation? GetNextToNotify(string isbn);
    List<TbReservation> GetReservationsToExpire(DateOnly today);
}
