using System.Net.Http.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpPostService : IPostService
{
    private readonly HttpClient client;

    public HttpPostService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<PostDto> AddPostAsync(CreatePostDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("posts", request);
        return await HttpHelper.ReadAsync<PostDto>(httpResponse);
    }

    public async Task<PostDto> GetPostAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"posts/{id}");
        return await HttpHelper.ReadAsync<PostDto>(httpResponse);
    }

    public async Task<IEnumerable<PostDto>> GetPostsAsync(string? titleContains = null, int? userId = null)
    {
        List<string> query = new();
        if (!string.IsNullOrWhiteSpace(titleContains))
        {
            query.Add($"title={Uri.EscapeDataString(titleContains)}");
        }
        if (userId.HasValue)
        {
            query.Add($"userId={userId.Value}");
        }

        string uri = query.Count == 0 ? "posts" : "posts?" + string.Join("&", query);
        HttpResponseMessage httpResponse = await client.GetAsync(uri);
        return await HttpHelper.ReadAsync<List<PostDto>>(httpResponse);
    }

    public async Task UpdatePostAsync(int id, UpdatePostDto request)
    {
        HttpResponseMessage httpResponse = await client.PutAsJsonAsync($"posts/{id}", request);
        await HttpHelper.EnsureSuccessAsync(httpResponse);
    }

    public async Task DeletePostAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.DeleteAsync($"posts/{id}");
        await HttpHelper.EnsureSuccessAsync(httpResponse);
    }
}
