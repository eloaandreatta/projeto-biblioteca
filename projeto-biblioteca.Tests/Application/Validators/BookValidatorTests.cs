using NUnit.Framework;
using pBiblioteca.Application.Validators;
using pBiblioteca.Models;

namespace projeto_biblioteca.Tests;

[TestFixture]
public class BookValidatorTests
{
    [Test]
    public void Should_Throw_When_AvailableQuantity_Is_Insufficient()
    {
        var book = new TbBook
        {
            Isbn = "123",
            Title = "Livro Teste",
            Author = "Autor",
            Publicationyear = 2024,
            Category = "Teste",
            Publisher = "Editora",
            Totalquantity = 2,
            Availablequantity = 2
        };

        Assert.Throws<InvalidOperationException>(() =>
            BookValidator.ValidateAvailableQuantity(book, 5)
        );
    }

    [Test]
    public void Should_Not_Throw_When_AvailableQuantity_Is_Enough()
    {
        var book = new TbBook
        {
            Isbn = "123",
            Title = "Livro Teste",
            Author = "Autor",
            Publicationyear = 2024,
            Category = "Teste",
            Publisher = "Editora",
            Totalquantity = 10,
            Availablequantity = 5
        };

        Assert.DoesNotThrow(() =>
            BookValidator.ValidateAvailableQuantity(book, 5)
        );
    }
}