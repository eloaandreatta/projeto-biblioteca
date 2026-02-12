namespace pBiblioteca.Models;

public class BookResponseDTO
{
    public string Isbn { get; set; } = null!;
    public string Titulo { get; set; } = null!;
    public string Autor { get; set; } = null!;
    public int AnoPublicacao { get; set; }
    public string Categoria { get; set; } = null!;
    public string Editora { get; set; } = null!;
    public int QuantidadeTotal { get; set; }
    public int QuantidadeDisponivel { get; set; }
}

public class CreateBookRequest
{
    public string Isbn { get; set; } = null!;
    public string Titulo { get; set; } = null!;
    public string Autor { get; set; } = null!;
    public int AnoPublicacao { get; set; }
    public string Categoria { get; set; } = null!;
    public string Editora { get; set; } = null!;
    public int QuantidadeTotal { get; set; }
    public int QuantidadeDisponivel { get; set; }
}
