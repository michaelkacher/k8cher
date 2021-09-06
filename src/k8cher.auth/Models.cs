public class RegisterRequest
{
    public string Email { get; set; } = String.Empty;
    public string Password { get; set; } = String.Empty;
}

public class ConfirmAccountRequest
{
    public string Token { get; set; } = String.Empty;
    public string UserId { get; set; } = String.Empty;
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

public enum ConfirmationResult
{
    SendConfirmationLink,
    SendAccountRequestMade,
    ConfirmationFailed,
    InvalidEmailFormat,
    InvalidPasswordFormat
}