using System.Net.Http.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpUserService : IUserService
{
    private readonly HttpClient client;

    public HttpUserService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<UserDto> AddUserAsync(CreateUserDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("users", request);
        return await HttpHelper.ReadAsync<UserDto>(httpResponse);
    }

    public async Task<UserDto> GetUserAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"users/{id}");
        return await HttpHelper.ReadAsync<UserDto>(httpResponse);
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync(string? usernameContains = null)
    {
        string uri = "users";
        if (!string.IsNullOrWhiteSpace(usernameContains))
        {
            uri += $"?username={Uri.EscapeDataString(usernameContains)}";
        }

        HttpResponseMessage httpResponse = await client.GetAsync(uri);
        return await HttpHelper.ReadAsync<List<UserDto>>(httpResponse);
    }

    public async Task UpdateUserAsync(int id, UpdateUserDto request)
    {
        HttpResponseMessage httpResponse = await client.PutAsJsonAsync($"users/{id}", request);
        await HttpHelper.EnsureSuccessAsync(httpResponse);
    }

    public async Task DeleteUserAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.DeleteAsync($"users/{id}");
        await HttpHelper.EnsureSuccessAsync(httpResponse);
    }
}
