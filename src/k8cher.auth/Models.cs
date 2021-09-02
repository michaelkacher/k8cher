public class RegisterRequest
{
    public User? User { get; set; }
    public string Password { get; set; } = String.Empty;
}

public class LoginRequest
{
    public string Email { get; set; } = String.Empty;
    public string Password { get; set; } = String.Empty;
}

public class LoginResponse
{
    public string Jwt { get; set; } = String.Empty;
}