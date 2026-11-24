using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using ApiContracts;
using Entities;

namespace WebApplication3.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepository;

    public PostsController(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }
    
    [HttpPost]
    public async Task<ActionResult<PostDto>> AddPost([FromBody] CreatePostDto request)
    {
        Post post = new()
        {
            Title = request.Title,
            Body = request.Body,
            UserId = request.UserId
        };

        Post created = await postRepository.AddAsync(post);

        PostDto dto = new()
        {
            Id = created.Id,
            Title = created.Title,
            Body = created.Body,
            UserId = created.UserId
        };

        return Created($"/Posts/{dto.Id}", dto);
    }
    
    // Parte 7: una sola consulta con Include + Select que trae el post,
    // el nombre del autor y sus comentarios (en vez de varias llamadas a la BD).
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetPost(int id)
    {
        PostDto? dto = await postRepository.GetMany()
            .Where(p => p.Id == id)
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Select(p => new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Body = p.Body,
                UserId = p.UserId,
                AuthorName = p.User.Username,
                Comments = p.Comments
                    .Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Body = c.Body,
                        UserId = c.UserId,
                        PostId = c.PostId
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (dto is null)
        {
            return NotFound($"Post with ID {id} was not found.");
        }

        return Ok(dto);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts(
        [FromQuery] string? title,
        [FromQuery] int? userId)
    {
        IQueryable<Post> query = postRepository.GetMany();

        if (!string.IsNullOrWhiteSpace(title))
        {
            string lower = title.ToLower();
            query = query.Where(post => post.Title.ToLower().Contains(lower));
        }

        if (userId.HasValue)
        {
            query = query.Where(post => post.UserId == userId.Value);
        }

        List<PostDto> posts = await query
            .Select(post => new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                UserId = post.UserId
            })
            .ToListAsync();

        return Ok(posts);
    }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdatePost(int id, [FromBody] UpdatePostDto request)
    {
        Post post = await postRepository.GetSingleAsync(id);

        post.Title = request.Title;
        post.Body = request.Body;

        await postRepository.UpdateAsync(post);

        return NoContent();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeletePost(int id)
    {
        await postRepository.DeleteAsync(id);

        return NoContent();
    }
}