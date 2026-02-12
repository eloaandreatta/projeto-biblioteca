using System;
using System.Collections.Generic;

namespace pBiblioteca.Models;

public partial class TbLoan
{
    public int Id { get; set; }

    public string UserCpf { get; set; } = null!;

    public string BookIsbn { get; set; } = null!;

    public DateOnly Loandate { get; set; }

    public DateOnly Duedate { get; set; }

    public DateOnly Returndate { get; set; }

    public bool Status { get; set; }

    public virtual TbBook BookIsbnNavigation { get; set; } = null!;

    public virtual ICollection<TbFine> TbFines { get; set; } = new List<TbFine>();

    public virtual TbUser UserCpfNavigation { get; set; } = null!;
}
