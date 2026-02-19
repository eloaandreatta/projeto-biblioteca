namespace pBiblioteca.Models;

public class ReservationResponseDTO
{
    public long Id { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;

    public DateTime ReservationDate { get; set; }
    public string Status { get; set; } = string.Empty;

    public DateTime? PickupDeadline { get; set; }

    // pra mostrar fila
    public int Position { get; set; }
}

public class CreateReservationRequest
{
    public string Cpf { get; set; } = default!;
    public string Isbn { get; set; } = default!;
}

