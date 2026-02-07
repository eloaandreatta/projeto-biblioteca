using System;
using System.Collections.Generic;

namespace pBiblioteca.Models;

public partial class TbBook
{
    public string Isbn { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public int Publicationyear { get; set; }

    public string Category { get; set; } = null!;

    public string Publisher { get; set; } = null!;

    public int Totalquantity { get; set; }

    public int Availablequantity { get; set; }

    public virtual ICollection<TbLoan> TbLoans { get; set; } = new List<TbLoan>();
}
