using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class PostFileRepository : IPostRepository
{
    private readonly string filePath = "posts.json";

    public PostFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    public async Task<Post> AddAsync(Post post)
    {
        List<Post> posts = await LoadPostsAsync();

        int maxId = posts.Count > 0
            ? posts.Max(p => p.Id)
            : 0;

        post.Id = maxId + 1;

        posts.Add(post);

        await SavePostsAsync(posts);

        return post;
    }

    public async Task UpdateAsync(Post post)
    {
        List<Post> posts = await LoadPostsAsync();

        int index = posts.FindIndex(p => p.Id == post.Id);

        if (index == -1)
        {
            throw new KeyNotFoundException(
                $"Post with ID {post.Id} was not found.");
        }

        posts[index] = post;

        await SavePostsAsync(posts);
    }

    public async Task DeleteAsync(int id)
    {
        List<Post> posts = await LoadPostsAsync();

        Post? post = posts.FirstOrDefault(p => p.Id == id);

        if (post is null)
        {
            throw new KeyNotFoundException(
                $"Post with ID {id} was not found.");
        }

        posts.Remove(post);

        await SavePostsAsync(posts);
    }

    public async Task<Post> GetSingleAsync(int id)
    {
        List<Post> posts = await LoadPostsAsync();

        Post? post = posts.FirstOrDefault(p => p.Id == id);

        if (post is null)
        {
            throw new KeyNotFoundException(
                $"Post with ID {id} was not found.");
        }

        return post;
    }

    public IQueryable<Post> GetMany()
    {
        string json = File.ReadAllTextAsync(filePath).Result;

        List<Post> posts =
            JsonSerializer.Deserialize<List<Post>>(json)
            ?? new List<Post>();

        return posts.AsQueryable();
    }

    private async Task<List<Post>> LoadPostsAsync()
    {
        string json = await File.ReadAllTextAsync(filePath);

        return JsonSerializer.Deserialize<List<Post>>(json)
               ?? new List<Post>();
    }

    private async Task SavePostsAsync(List<Post> posts)
    {
        string json = JsonSerializer.Serialize(posts);

        await File.WriteAllTextAsync(filePath, json);
    }
}