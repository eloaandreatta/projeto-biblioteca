using Microsoft.EntityFrameworkCore;
using pBiblioteca.Models;

public class ReservationRepository : IReservationRepository
{
    private readonly PostgresContext _dbContext;

    public ReservationRepository(PostgresContext context)
    {
        _dbContext = context;
    }

    public TbReservation? GetById(int id)
    {
        int rid = checked((int)id);
        return _dbContext.TbReservations.FirstOrDefault(r => r.Id == rid);
    }

    public TbReservation? GetActiveReservationForUserAndBook(string cpf, string isbn)
    {
        // Reserva não tem ISBN direto: relacionamento N:N em TbReservationBook
        return _dbContext.TbReservationBooks
            .AsNoTracking()
            .Where(rb => rb.BookIsbn == isbn &&
                         rb.Reservation.UserCpf == cpf &&
                         rb.Reservation.Status == true)
            .OrderBy(rb => rb.Reservation.Reservationdate)
            .ThenBy(rb => rb.ReservationId)
            .Select(rb => rb.Reservation)
            .FirstOrDefault();
    }

    public List<TbReservation> GetActiveQueueByBook(string isbn)
    {
        return _dbContext.TbReservationBooks
            .AsNoTracking()
            .Where(rb => rb.BookIsbn == isbn && rb.Reservation.Status == true)
            .OrderBy(rb => rb.Reservation.Reservationdate)
            .ThenBy(rb => rb.ReservationId)
            .Select(rb => rb.Reservation)
            .ToList();
    }

    public int CountActiveBefore(string isbn, DateOnly reservationDate, int reservationId)
    {
        int rid = checked((int)reservationId);

        // Conta reservas ativas com data menor, ou mesma data porém Id menor
        return _dbContext.TbReservationBooks
            .AsNoTracking()
            .Where(rb => rb.BookIsbn == isbn && rb.Reservation.Status == true)
            .Count(rb =>
                rb.Reservation.Reservationdate < reservationDate ||
                (rb.Reservation.Reservationdate == reservationDate && rb.ReservationId < rid)
            );
    }

    public void CreateReservationWithBook(TbReservation reservation, string isbn)
    {
        using var tx = _dbContext.Database.BeginTransaction();

        _dbContext.TbReservations.Add(reservation);
        _dbContext.SaveChanges(); // gera reservation.Id

        var link = new TbReservationBook
        {
            ReservationId = reservation.Id,
            BookIsbn = isbn
        };

        _dbContext.TbReservationBooks.Add(link);
        _dbContext.SaveChanges();

        tx.Commit();
    }

    public void Update(TbReservation reservation)
    {
        _dbContext.TbReservations.Update(reservation);
        _dbContext.SaveChanges();
    }
}