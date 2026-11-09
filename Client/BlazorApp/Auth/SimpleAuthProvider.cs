using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using ApiContracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BlazorApp.Auth;

// Le dice a Blazor quién está logueado. Guarda el usuario en el sessionStorage
// del navegador para que no se pierda al refrescar la página.
public class SimpleAuthProvider : AuthenticationStateProvider
{
    private const string StorageKey = "currentUser";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient httpClient;
    private readonly IJSRuntime jsRuntime;

    // "Caché primaria": así no leemos el sessionStorage cada vez
    private ClaimsPrincipal? cachedPrincipal;

    public SimpleAuthProvider(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        this.httpClient = httpClient;
        this.jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (cachedPrincipal is not null)
        {
            return new AuthenticationState(cachedPrincipal);
        }

        string? userAsJson;
        try
        {
            userAsJson = await jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", StorageKey);
        }
        catch (InvalidOperationException)
        {
            // JavaScript todavía no está disponible -> nadie logueado de momento
            return new AuthenticationState(new ClaimsPrincipal());
        }

        if (string.IsNullOrEmpty(userAsJson))
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }

        UserDto? userDto = JsonSerializer.Deserialize<UserDto>(userAsJson, JsonOptions);
        if (userDto is null)
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }

        cachedPrincipal = CreateClaimsPrincipal(userDto);
        return new AuthenticationState(cachedPrincipal);
    }

    public async Task LoginAsync(string username, string password)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "auth/login",
            new LoginRequest(username, password));

        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(string.IsNullOrWhiteSpace(content)
                ? $"{(int)response.StatusCode} {response.ReasonPhrase}"
                : content);
        }

        UserDto userDto = JsonSerializer.Deserialize<UserDto>(content, JsonOptions)!;

        // Guardar en el navegador
        string serialisedData = JsonSerializer.Serialize(userDto);
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", StorageKey, serialisedData);

        cachedPrincipal = CreateClaimsPrincipal(userDto);
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(cachedPrincipal)));
    }

    public async Task LogoutAsync()
    {
        await jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", StorageKey);
        cachedPrincipal = null;
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
    }

    private static ClaimsPrincipal CreateClaimsPrincipal(UserDto userDto)
    {
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Name, userDto.Username),
            new Claim("Id", userDto.Id.ToString()),
            // Aquí puedes añadir más claims, p. ej. new Claim("IsAdmin", ...)
        };

        ClaimsIdentity identity = new(claims, "apiauth");
        return new ClaimsPrincipal(identity);
    }
}
