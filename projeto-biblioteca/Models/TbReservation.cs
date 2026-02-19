using System;
using System.Collections.Generic;

namespace pBiblioteca.Models;

public partial class TbReservation
{
    public int Id { get; set; }

    public string UserCpf { get; set; } = null!;

    public DateOnly Reservationdate { get; set; }

    public DateOnly Notifieddate { get; set; }

    public DateOnly Expirationdate { get; set; }

    public bool Status { get; set; }

    public virtual TbUser UserCpfNavigation { get; set; } = null!;
}
