using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace WebApplication3.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepository;

    public UsersController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser([FromBody] CreateUserDto request)
    {
        // Para que el login funcione, el username tiene que ser único
        string lowerName = request.Username.ToLower();
        bool usernameTaken = await userRepository.GetMany()
            .AnyAsync(u => u.Username.ToLower() == lowerName);
        if (usernameTaken)
        {
            return Conflict($"Username '{request.Username}' is already taken");
        }

        User user = new()
        {
            Username = request.Username,
            Password = request.Password
        };

        User created = await userRepository.AddAsync(user);

        UserDto dto = new()
        {
            Id = created.Id,
            Username = created.Username
        };

        return Created($"/Users/{dto.Id}", dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        User user = await userRepository.GetSingleAsync(id);

        UserDto dto = new()
        {
            Id = user.Id,
            Username = user.Username
        };

        return Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] string? username)
    {
        IQueryable<User> query = userRepository.GetMany();

        if (!string.IsNullOrWhiteSpace(username))
        {
            string lower = username.ToLower();
            query = query.Where(user => user.Username.ToLower().Contains(lower));
        }

        List<UserDto> users = await query
            .Select(user => new UserDto
            {
                Id = user.Id,
                Username = user.Username
            })
            .ToListAsync();

        return Ok(users);
    }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateUser(int id, [FromBody] UpdateUserDto request)
    {
        User user = await userRepository.GetSingleAsync(id);

        user.Username = request.Username;
        user.Password = request.Password;

        await userRepository.UpdateAsync(user);

        return NoContent();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        await userRepository.DeleteAsync(id);

        return NoContent();
    }
}