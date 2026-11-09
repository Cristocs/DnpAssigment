using System.Text.Json;

namespace BlazorApp.Services;

// Código común a todos los servicios: leer la respuesta y lanzar excepción si falló
internal static class HttpHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<T> ReadAsync<T>(HttpResponseMessage httpResponse)
    {
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(ErrorMessage(httpResponse, response));
        }

        return JsonSerializer.Deserialize<T>(response, JsonOptions)!;
    }

    public static async Task EnsureSuccessAsync(HttpResponseMessage httpResponse)
    {
        if (!httpResponse.IsSuccessStatusCode)
        {
            string response = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception(ErrorMessage(httpResponse, response));
        }
    }

    private static string ErrorMessage(HttpResponseMessage httpResponse, string body)
    {
        if (string.IsNullOrWhiteSpace(body) || body.Length > 300)
        {
            // Respuesta vacía o una página de error enorme: mostramos solo el código
            return $"{(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase}";
        }

        return body.Trim('"');
    }
}
