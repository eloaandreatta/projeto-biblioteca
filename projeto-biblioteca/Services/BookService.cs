using pBiblioteca.Models;

public class BookService : IBookService
{
    private IBookRepository _repository;

    public BookService(IBookRepository repository)
    {
        _repository = repository;
    }

    public List<BookResponseDTO> GetBooks()
    {
        List<TbBook> tbBooks = _repository.SelectBooks();

        List<BookResponseDTO> booksDTO = new List<BookResponseDTO>();

        foreach (TbBook tbBook in tbBooks)
        {
            BookResponseDTO bookRetorno = new BookResponseDTO();
            bookRetorno.Isbn = tbBook.Isbn;
            bookRetorno.Titulo = tbBook.Title;
            bookRetorno.Autor = tbBook.Author;
            bookRetorno.AnoPublicacao = tbBook.Publicationyear;
            bookRetorno.Categoria = tbBook.Category;
            bookRetorno.Editora = tbBook.Publisher;
            bookRetorno.QuantidadeTotal = tbBook.Totalquantity;
            bookRetorno.QuantidadeDisponivel = tbBook.Availablequantity;

            booksDTO.Add(bookRetorno);
        }

        return booksDTO;
    }

    public BookResponseDTO? GetBookByIsbn(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            return null;

        TbBook? tbBook = _repository.GetBookByIsbn(isbn.Trim());

        if (tbBook == null)
            return null;

        return new BookResponseDTO
        {
            Isbn = tbBook.Isbn,
            Titulo = tbBook.Title,
            Autor = tbBook.Author,
            AnoPublicacao = tbBook.Publicationyear,
            Categoria = tbBook.Category,
            Editora = tbBook.Publisher,
            QuantidadeTotal = tbBook.Totalquantity,
            QuantidadeDisponivel = tbBook.Availablequantity
        };
    }

    public string CreateBook(CreateBookRequest request)
    {
        
        if (request == null) return "error";

        if (string.IsNullOrWhiteSpace(request.Isbn)) return "error";
        if (string.IsNullOrWhiteSpace(request.Titulo)) return "error";
        if (string.IsNullOrWhiteSpace(request.Autor)) return "error";
        if (string.IsNullOrWhiteSpace(request.Categoria)) return "error";
        if (string.IsNullOrWhiteSpace(request.Editora)) return "error";

        if (request.AnoPublicacao <= 0) return "error";
        if (request.QuantidadeTotal < 0) return "error";
        if (request.QuantidadeDisponivel < 0) return "error";
        if (request.QuantidadeDisponivel > request.QuantidadeTotal) return "error";

        string isbn = request.Isbn.Trim();

        // impede duplicado por ISBN
        TbBook? exists = _repository.GetBookByIsbn(isbn);
        if (exists != null) return "error";

        _repository.InsertBook(
            isbn,
            request.Titulo.Trim(),
            request.Autor.Trim(),
            request.AnoPublicacao,
            request.Categoria.Trim(),
            request.Editora.Trim(),
            request.QuantidadeTotal,
            request.QuantidadeDisponivel
        );

        return "";
    }
}
