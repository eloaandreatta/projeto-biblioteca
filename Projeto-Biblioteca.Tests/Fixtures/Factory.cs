using pBiblioteca.DTO;
using pBiblioteca.Models;

namespace Projeto_Biblioteca.Tests.Fixtures.TestData;

public static class Factory
{
    public static TbUser User(string cpf = TestData.CpfOk, bool active = true) =>
        new TbUser { Cpf = cpf, Active = active };

    public static TbBook Book(string isbn = TestData.IsbnOk, int available = 0, int total = 10) =>
        new TbBook
        {
            Isbn = isbn,
            Title = "Titulo",
            Author = "Autor",
            Publicationyear = 2020,
            Category = "Categoria",
            Publisher = "Editora",
            Totalquantity = total,
            Availablequantity = available
        };

    public static TbReservation Reservation(int id = 1, string cpf = TestData.CpfOk, bool status = true, DateOnly? date = null) =>
        new TbReservation
        {
            Id = id,
            UserCpf = cpf,
            Status = status,
            Reservationdate = date ?? DateOnly.FromDateTime(DateTime.Today),
            Notifieddate = DateOnly.MinValue,
            Expirationdate = DateOnly.MinValue
        };

    public static CreateBookRequest CreateBookRequest(
        string isbn = TestData.IsbnOk,
        string title = "Titulo",
        string author = "Autor",
        int publicationYear = 2020,
        string category = "Categoria",
        string publisher = "Editora",
        int totalQuantity = 10,
        int availableQuantity = 5
    ) => new CreateBookRequest
    {
        Isbn = isbn,
        Title = title,
        Author = author,
        PublicationYear = publicationYear,
        Category = category,
        Publisher = publisher,
        TotalQuantity = totalQuantity,
        AvailableQuantity = availableQuantity
    };

    public static CreateReservationRequest CreateReservationRequest(
        string cpf = TestData.CpfOk,
        string isbn = TestData.IsbnOk
    ) => new CreateReservationRequest
    {
        UserCpf = cpf,
        BookIsbn = isbn
    };
}