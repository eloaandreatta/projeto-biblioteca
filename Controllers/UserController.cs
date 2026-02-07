using pBiblioteca.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
 
    [HttpPatch("{id}", Name = "UpdateUserPassword")]
    public IActionResult UpdateUser([FromRoute] Guid id, [FromBody] UpdateUserRequest request){
       string error = _service.UpdateUserPassword(id, request.Senha);

        if(error == "error"){
            return BadRequest();
        }
        
        return Ok();
    }
}