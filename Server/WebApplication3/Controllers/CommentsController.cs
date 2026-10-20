using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using ApiContracts;
using Entities;

namespace WebApplication3.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepository;

    public CommentsController(ICommentRepository commentRepository)
    {
        this.commentRepository = commentRepository;
    }
    
    [HttpPost]
    public async Task<ActionResult<CommentDto>> AddComment([FromBody] CreateCommentDto request)
    {
        Comment comment = new()
        {
            Body = request.Body,
            UserId = request.UserId,
            PostId = request.PostId
        };

        Comment created = await commentRepository.AddAsync(comment);

        CommentDto dto = new()
        {
            Id = created.Id,
            Body = created.Body,
            UserId = created.UserId,
            PostId = created.PostId
        };

        return Created($"/Comments/{dto.Id}", dto);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetComment(int id)
    {
        Comment comment = await commentRepository.GetSingleAsync(id);

        CommentDto dto = new()
        {
            Id = comment.Id,
            Body = comment.Body,
            UserId = comment.UserId,
            PostId = comment.PostId
        };

        return Ok(dto);
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<CommentDto>> GetComments(
        [FromQuery] int? userId,
        [FromQuery] int? postId)
    {
        IQueryable<Comment> query = commentRepository.GetMany();

        if (userId.HasValue)
        {
            query = query.Where(comment => comment.UserId == userId.Value);
        }

        if (postId.HasValue)
        {
            query = query.Where(comment => comment.PostId == postId.Value);
        }

        List<CommentDto> comments = query
            .Select(comment => new CommentDto
            {
                Id = comment.Id,
                Body = comment.Body,
                UserId = comment.UserId,
                PostId = comment.PostId
            })
            .ToList();

        return Ok(comments);
    }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateComment(int id, [FromBody] UpdateCommentDto request)
    {
        Comment comment = await commentRepository.GetSingleAsync(id);

        comment.Body = request.Body;

        await commentRepository.UpdateAsync(comment);

        return NoContent();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteComment(int id)
    {
        await commentRepository.DeleteAsync(id);

        return NoContent();
    }
}