using System;
using System.Collections.Generic;

namespace pBiblioteca.Models;

public partial class TbFine
{
    public int Id { get; set; }

    public string UserCpf { get; set; } = null!;

    public int LoanId { get; set; }

    public decimal Amount { get; set; }

    public int Dayslate { get; set; }

    public bool Ispaid { get; set; }

    public decimal Dailyrate { get; set; }

    public DateOnly Paymentdate { get; set; }

    public virtual TbLoan Loan { get; set; } = null!;

    public virtual TbUser UserCpfNavigation { get; set; } = null!;
}

