using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApplication3.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository userRepository;

    public AuthController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    // POST /auth/login
    [HttpPost("login")]
    public ActionResult<UserDto> Login([FromBody] LoginRequest request)
    {
        // 1. Buscar el usuario por su username
        User? user = userRepository.GetMany()
            .FirstOrDefault(u => u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase));

        // 2. Si no existe -> 401
        if (user is null)
        {
            return Unauthorized("User not found");
        }

        // 3. Si la contraseña no coincide -> 401
        if (user.Password != request.Password)
        {
            return Unauthorized("Incorrect password");
        }

        // 4. Convertir a UserDto (sin la contraseña) y 5. devolverlo
        UserDto dto = new()
        {
            Id = user.Id,
            Username = user.Username
        };

        return Ok(dto);
    }
}
