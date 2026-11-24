namespace ApiContracts;

public class PostDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public int UserId { get; set; }

    // Parte 7: solo se rellenan en GET /Posts/{id} (consulta con Include)
    public string? AuthorName { get; set; }
    public List<CommentDto>? Comments { get; set; }
}