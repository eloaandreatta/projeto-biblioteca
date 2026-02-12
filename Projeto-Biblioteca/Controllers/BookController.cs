using pBiblioteca.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace pBiblioteca.Controllers;

// Responsavel apenas por receber e enviar dados pro cliente
// Conecta ao Service
[ApiController]
[Route("[controller]")]
public class BookController : ControllerBase
{
    private IBookService _service;

    public BookController(IBookService service)
    {
        _service = service;
    }

    //GET /Book
    [HttpGet(Name = "GetAllBooks")]
    public List<BookResponseDTO> Get()
    {
        return _service.GetBooks();
    }
 
    // GET /Book/{Isbn}
    [HttpGet("{Isbn}", Name = "GetBookByIsbn")]
    public IActionResult GetByIsbn([FromRoute] string Isbn)
    {
        BookResponseDTO? book = _service.GetBookByIsbn(Isbn);

        if (book == null)
            return NotFound();

        return Ok(book);
    }
    
    // POST /Book
    [HttpPost(Name = "CreateBook")]
    public IActionResult Post([FromBody] CreateBookRequest request){
       string error = _service.CreateBook(request);

        if(error == "error"){
            return BadRequest();
        }
        
        return Ok();
    }

}