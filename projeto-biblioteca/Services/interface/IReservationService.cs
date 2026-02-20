using pBiblioteca.Models;

namespace pBiblioteca.Services;

public interface IReservationService
{
    string CreateReservation(CreateReservationRequest request);
    string CancelReservation(int reservationId);
    int GetUserPosition(string isbn, string cpf);
    List<ReservationResponseDTO> GetQueue(string isbn);

    // NOVOS (para regras de notificação/expiração)
    void NotifyNextIfAny(string isbn, DateOnly notifiedDate, DateOnly expirationDate);
    int ExpireOverdueReservations(DateOnly today);
}
