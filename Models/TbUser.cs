using System;
using System.Collections.Generic;

namespace pBiblioteca.Models;

public partial class TbUser
{
    public string Cpf { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Telephone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime Registrationdate { get; set; }

    public int Loanlimits { get; set; }

    public bool Active { get; set; }

    public virtual ICollection<TbFine> TbFines { get; set; } = new List<TbFine>();

    public virtual ICollection<TbLoan> TbLoans { get; set; } = new List<TbLoan>();

    public virtual ICollection<TbReservation> TbReservations { get; set; } = new List<TbReservation>();
}
