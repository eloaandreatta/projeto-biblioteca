using Microsoft.EntityFrameworkCore;
using pBiblioteca.Models;

public class BookRepository : IBookRepository
{
    private PostgresContext _context;

    public BookRepository(PostgresContext context)
    {
        _context = context;
    }

    public List<TbBook> SelectBooks()
    {
        return _context.TbBooks.AsNoTracking().ToList();
    }

    public TbBook? GetBookByIsbn(string isbn)
    {
        return _context.TbBooks.AsNoTracking().FirstOrDefault(b => b.Isbn == isbn);
    }

    public void InsertBook(
        string isbn,
        string title,
        string author,
        int publicationYear,
        string category,
        string publisher,
        int totalQuantity,
        int availableQuantity
    )
    {
        TbBook book = new TbBook
        {
            Isbn = isbn,
            Title = title,
            Author = author,
            Publicationyear = publicationYear,
            Category = category,
            Publisher = publisher,
            Totalquantity = totalQuantity,
            Availablequantity = availableQuantity
        };

        _context.TbBooks.Add(book);
        _context.SaveChanges();
    }
}
