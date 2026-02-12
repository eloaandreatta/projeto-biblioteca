using pBiblioteca.Models;

public interface IBookRepository
{
    public List<TbBook> SelectBooks();
    public TbBook? GetBookByIsbn(string isbn);
    public void InsertBook(
        string isbn,
        string title,
        string author,
        int publicationYear,
        string category,
        string publisher,
        int totalQuantity,
        int availableQuantity
    );
}
