namespace pBiblioteca.Models;

public class BookResponseDTO
{
    public string Isbn { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public int PublicationYear { get; set; }
    public string Category { get; set; } = null!;
    public string Publisher { get; set; } = null!;
    public int TotalQuantity { get; set; }
    public int AvailableQuantity { get; set; }
}

public class CreateBookRequest
{
    public string Isbn { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public int PublicationYear { get; set; }
    public string Category { get; set; } = null!;
    public string Publisher { get; set; } = null!;
    public int TotalQuantity { get; set; }
    public int AvailableQuantity { get; set; }
}
