using System;
using System.Collections.Generic;

namespace pBiblioteca.Models;

public partial class TbReservationBook
{
    public int ReservationId { get; set; }

    public string BookIsbn { get; set; } = null!;

    public virtual TbBook BookIsbnNavigation { get; set; } = null!;

    public virtual TbReservation Reservation { get; set; } = null!;
}
