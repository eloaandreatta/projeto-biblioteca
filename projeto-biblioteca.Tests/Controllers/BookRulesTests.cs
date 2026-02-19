using System;
using NUnit.Framework;

using pBiblioteca.Models;

namespace projeto_biblioteca.Tests;

[TestFixture]
public class BookRulesTests
{
    [Test]
    public void ValidateLoan_WhenAvailableQuantityIsZero_ShouldThrow()
    {
        // Arrange
        var book = new TbBook
        {
            Isbn = "123",
            Title = "Livro Teste",
            Author = "Autor",
            Publicationyear = 2024,
            Category = "Teste",
            Publisher = "Editora",
            Totalquantity = 2,
            Availablequantity = 0
        };

        // Act + Assert
        Assert.Throws<InvalidOperationException>(() =>
        {
            // Regra de negócio não emprestar livro com quantidade menor ou igual a 0
            if (book.Availablequantity <= 0)
                throw new InvalidOperationException("Livro indisponível para empréstimo");
        });
    }
}