using pBiblioteca.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pBiblioteca.DTO;

namespace pBiblioteca.Controllers;

// Responsavel apenas por receber e enviar dados pro cliente
// Conecta ao Service
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpGet(Name = "GetAllUsers")]
    public List<UserResponseDTO> Get()
    {
        return _service.GetUsers();
    }

    [HttpPut("{cpf}")]
    public IActionResult UpdateUser(string cpf, [FromBody] UpdateUserRequestDTO request)
    {
    string result = _service.UpdateUser(cpf, request);

    if (result == "error")
        return NotFound();
        return Ok();
    }


}