using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class CommentFileRepository : ICommentRepository
{
    private readonly string filePath = "comments.json";

    public CommentFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        List<Comment> comments = await LoadCommentsAsync();

        int maxId = comments.Count > 0
            ? comments.Max(c => c.Id)
            : 0;

        comment.Id = maxId + 1;

        comments.Add(comment);

        await SaveCommentsAsync(comments);

        return comment;
    }

    public async Task UpdateAsync(Comment comment)
    {
        List<Comment> comments = await LoadCommentsAsync();

        int index = comments.FindIndex(c => c.Id == comment.Id);

        if (index == -1)
        {
            throw new KeyNotFoundException(
                $"Comment with ID {comment.Id} was not found.");
        }

        comments[index] = comment;

        await SaveCommentsAsync(comments);
    }

    public async Task DeleteAsync(int id)
    {
        List<Comment> comments = await LoadCommentsAsync();

        Comment? comment = comments.FirstOrDefault(c => c.Id == id);

        if (comment is null)
        {
            throw new KeyNotFoundException(
                $"Comment with ID {id} was not found.");
        }

        comments.Remove(comment);

        await SaveCommentsAsync(comments);
    }

    public async Task<Comment> GetSingleAsync(int id)
    {
        List<Comment> comments = await LoadCommentsAsync();

        Comment? comment = comments.FirstOrDefault(c => c.Id == id);

        if (comment is null)
        {
            throw new KeyNotFoundException(
                $"Comment with ID {id} was not found.");
        }

        return comment;
    }

    public IQueryable<Comment> GetMany()
    {
        string json = File.ReadAllTextAsync(filePath).Result;

        List<Comment> comments =
            JsonSerializer.Deserialize<List<Comment>>(json)
            ?? new List<Comment>();

        return comments.AsQueryable();
    }

    private async Task<List<Comment>> LoadCommentsAsync()
    {
        string json = await File.ReadAllTextAsync(filePath);

        return JsonSerializer.Deserialize<List<Comment>>(json)
               ?? new List<Comment>();
    }

    private async Task SaveCommentsAsync(List<Comment> comments)
    {
        string json = JsonSerializer.Serialize(comments);

        await File.WriteAllTextAsync(filePath, json);
    }
}