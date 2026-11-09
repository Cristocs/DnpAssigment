using ApiContracts;

namespace BlazorApp.Services;

public interface IPostService
{
    public Task<PostDto> AddPostAsync(CreatePostDto request);
    public Task<PostDto> GetPostAsync(int id);
    public Task<IEnumerable<PostDto>> GetPostsAsync(string? titleContains = null, int? userId = null);
    public Task UpdatePostAsync(int id, UpdatePostDto request);
    public Task DeletePostAsync(int id);
}
