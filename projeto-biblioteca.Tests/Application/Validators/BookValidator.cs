using pBiblioteca.Models;

namespace pBiblioteca.Application.Validators;

public static class BookValidator
{
    public static void ValidateAvailableQuantity(TbBook book, int requestedQuantity)
    {
        if (book is null)
            throw new ArgumentNullException(nameof(book));

        if (requestedQuantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(requestedQuantity), "Quantidade deve ser maior que zero.");

        if (book.Availablequantity < requestedQuantity)
            throw new InvalidOperationException("Quantidade disponível insuficiente para o empréstimo.");
    }
}