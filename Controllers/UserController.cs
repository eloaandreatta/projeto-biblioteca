using pBiblioteca.Models;
using Microsoft.AspNetCore.Mvc;
using pBiblioteca.DTO;

namespace pBiblioteca.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public List<UserResponseDTO> Get()
    {
        return _service.GetUsers();
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserRequestDTO request)
    {
        string result = _service.CreateUser(request);

        if (result.Contains("sucesso"))
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPut("{cpf}")]
    public IActionResult UpdateUser(string cpf, [FromBody] UpdateUserRequestDTO request)
    {
        string result = _service.UpdateUser(cpf, request);

        if (result.Contains("não encontrado"))
            return NotFound(result);

        return Ok(result);
    }
}

