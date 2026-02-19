using pBiblioteca.Models;
public interface IReservationService
{
    string CreateReservation(CreateReservationRequest request);
    string CancelReservation(long reservationId);
    int GetUserPosition(string isbn, string cpf);
    List<ReservationResponseDTO> GetQueue(string isbn);
}