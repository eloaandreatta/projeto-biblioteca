using pBiblioteca.Models;
namespace pBiblioteca.Services;
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
            bookRetorno.Title = tbBook.Title;
            bookRetorno.Author = tbBook.Author;
            bookRetorno.PublicationYear = tbBook.Publicationyear;
            bookRetorno.Category = tbBook.Category;
            bookRetorno.Publisher = tbBook.Publisher;
            bookRetorno.TotalQuantity = tbBook.Totalquantity;
            bookRetorno.AvailableQuantity = tbBook.Availablequantity;

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
            Title = tbBook.Title,
            Author = tbBook.Author,
            PublicationYear = tbBook.Publicationyear,
            Category = tbBook.Category,
            Publisher = tbBook.Publisher,
            TotalQuantity = tbBook.Totalquantity,
            AvailableQuantity = tbBook.Availablequantity
        };
    }

    public string CreateBook(CreateBookRequest request)
    {
        
        if (request == null) return "error";

        if (string.IsNullOrWhiteSpace(request.Isbn)) return "error";
        if (string.IsNullOrWhiteSpace(request.Title)) return "error";
        if (string.IsNullOrWhiteSpace(request.Author)) return "error";
        if (string.IsNullOrWhiteSpace(request.Category)) return "error";
        if (string.IsNullOrWhiteSpace(request.Publisher)) return "error";

        if (request.PublicationYear <= 0) return "error";
        if (request.TotalQuantity < 0) return "error";
        if (request.AvailableQuantity < 0) return "error";
        if (request.AvailableQuantity > request.TotalQuantity) return "error";


        string isbn = request.Isbn.Trim();

        // impede duplicado por ISBN
        TbBook? exists = _repository.GetBookByIsbn(isbn);
        if (exists != null) return "error";

        _repository.InsertBook(
            isbn,
            request.Title.Trim(),
            request.Author.Trim(),
            request.PublicationYear,
            request.Category.Trim(),
            request.Publisher.Trim(),
            request.TotalQuantity,
            request.AvailableQuantity
        );

        return "";
    }
}
