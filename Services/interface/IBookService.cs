using pBiblioteca.Models;

public interface IBookService
{
    public List<BookResponseDTO> GetBooks();
    public BookResponseDTO? GetBookByIsbn(string isbn);
    public string CreateBook(CreateBookRequest request);
}