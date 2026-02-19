namespace pBiblioteca.Models;

public class ReservationResponseDTO
{
    public int ReservationId { get; set; }
    public string UserCpf { get; set; } = string.Empty;
    public string BookIsbn { get; set; } = string.Empty;

    public DateOnly ReservationDate { get; set; }
    public bool Status { get; set; }
    public string StatusText { get; set; } = string.Empty;
    public DateOnly NotifiedDate { get; set; }
    public DateOnly ExpirationDate { get; set; }

    // posição na fila
    public int Position { get; set; }
}

public class CreateReservationRequest
{
    public string UserCpf { get; set; } = default!;
    public string BookIsbn { get; set; } = default!;
  
}

