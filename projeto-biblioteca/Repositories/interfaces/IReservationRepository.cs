using pBiblioteca.Models;

public interface IReservationRepository
{
    TbReservation? GetById(int id);

    /// Retorna uma reserva ativa (Status = true) do usuário para um livro.
    TbReservation? GetActiveReservationForUserAndBook(string cpf, string isbn);

    /// Fila de reservas ativas (Status = true) para um livro, ordenada por data/Id.
    List<TbReservation> GetActiveQueueByBook(string isbn);

    /// Quantas reservas ativas existem antes desta reserva na fila do livro.
    int CountActiveBefore(string isbn, DateOnly reservationDate, int reservationId);

    /// Cria a reserva e registra o vínculo com o livro.
    void CreateReservationWithBook(TbReservation reservation, string isbn);

    void Update(TbReservation reservation);
}
