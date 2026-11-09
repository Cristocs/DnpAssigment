namespace ApiContracts;

// Parte 6: datos que manda Blazor a POST /auth/login
public record LoginRequest(string Username, string Password);
