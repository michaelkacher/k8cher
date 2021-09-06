using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);

var daprHttpPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3600";
builder.Services.AddDaprClient(builder => builder.UseHttpEndpoint($"http://localhost:{daprHttpPort}"));

var connectionString = builder.Configuration.GetValue<string>("pg-connection-string");
builder.Services.AddDbContext<AuthContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddIdentity<User, Role>(o =>
        {
            o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            o.Lockout.MaxFailedAccessAttempts = 5;
            o.Lockout.AllowedForNewUsers = true;
            o.SignIn.RequireConfirmedAccount = true;
        }
    ).AddEntityFrameworkStores<AuthContext>();

builder.Services.AddScoped<AuthService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "K8cher Auth", Description = "OpenAPI specification for Auth Service", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger(c => c.RouteTemplate = "auth/swagger/{documentName}/swagger.json");
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/auth/swagger/v1/swagger.json", "K8cher Auth v1");
    c.RoutePrefix = "auth/swagger";
});

app.MapPost("/auth/register", async (RegisterRequest registerRequest, AuthService authService) =>
{
    var user = new User() { UserName = registerRequest.Email, Email = registerRequest.Email };
    var result = await authService.RegisterUser(user, registerRequest.Password);

    try
    {
        if (result == ConfirmationResult.SendConfirmationLink)
        {
            await authService.SendConfirmAccountEmail(user);
        }
        else if (result == ConfirmationResult.SendConfirmationLink)
        {
            await authService.SendConfirmAccountEmail(user);
        }
        else
        {
            return Results.BadRequest(result);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error sending e-mail: {ex.Message}");
        return Results.BadRequest("Error sending e-mail, please try again");
    }

    return Results.Ok();
});


app.MapPost("/auth/ConfirmAccount", async (ConfirmAccountRequest confirmAccountRequest, UserManager<User> userManager) =>
{
    try
    {
        var user = await userManager.FindByIdAsync(confirmAccountRequest.UserId);
        var result = await userManager.ConfirmEmailAsync(user, confirmAccountRequest.Token);
        if (result.Succeeded)
        {
            return Results.Ok();
        }
        else
        {
            return Results.BadRequest("TokenExpired");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error sending e-mail: {ex.Message}");
        return Results.BadRequest("Error sending e-mail");
    }
});

// TODO - mbk: Forgot password

// TODO - mbk: Reset password
// ResetPasswordAsync(TUser user, string token, string newPassword)

// TODO - mbk: Change password
// ChangePasswordAsync(TUser user, string currentPassword, string newPassword);

app.MapPost("/auth/login", async (LoginRequest loginRequest, UserManager<User> userManager, SignInManager<User> signInManager) =>
{
    var result = await signInManager.PasswordSignInAsync(
                loginRequest.Email,
                loginRequest.Password,
                false,
                false);

    if (result.Succeeded)
    {
        var user = await userManager.FindByEmailAsync(loginRequest.Email);

        var signingKey = Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("signing-key"));
        var securityKey = new SymmetricSecurityKey(signingKey);

        var handler = new JsonWebTokenHandler();
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                        new Claim("id", user.Id.ToString()),
                        new Claim("UserRole", "admin"),
                        new Claim("UserRole", "rawr"),
            }),
            Expires = DateTime.UtcNow.AddDays(1),
            Issuer = builder.Configuration.GetValue<string>("jwt-issuer"),
            Audience = builder.Configuration.GetValue<string>("jwt-audience"),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var loginResponse = new LoginResponse() { Jwt = handler.CreateToken(descriptor) };

        Console.WriteLine($"successful login {user.Email} of id {user.Id}");

        return Results.Ok(loginResponse);
    }

    Console.WriteLine($"Login failure: {loginRequest.Email}");

    return Results.BadRequest("Invalid login");
});

app.Run();
