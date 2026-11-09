using System.Net.Http.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpCommentService : ICommentService
{
    private readonly HttpClient client;

    public HttpCommentService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<CommentDto> AddCommentAsync(CreateCommentDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("comments", request);
        return await HttpHelper.ReadAsync<CommentDto>(httpResponse);
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsAsync(int? postId = null, int? userId = null)
    {
        List<string> query = new();
        if (postId.HasValue)
        {
            query.Add($"postId={postId.Value}");
        }
        if (userId.HasValue)
        {
            query.Add($"userId={userId.Value}");
        }

        string uri = query.Count == 0 ? "comments" : "comments?" + string.Join("&", query);
        HttpResponseMessage httpResponse = await client.GetAsync(uri);
        return await HttpHelper.ReadAsync<List<CommentDto>>(httpResponse);
    }

    public async Task UpdateCommentAsync(int id, UpdateCommentDto request)
    {
        HttpResponseMessage httpResponse = await client.PutAsJsonAsync($"comments/{id}", request);
        await HttpHelper.EnsureSuccessAsync(httpResponse);
    }

    public async Task DeleteCommentAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.DeleteAsync($"comments/{id}");
        await HttpHelper.EnsureSuccessAsync(httpResponse);
    }
}
