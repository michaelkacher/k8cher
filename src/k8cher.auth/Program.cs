var builder = WebApplication.CreateBuilder(args);

var daprClient = new DaprClientBuilder().UseHttpEndpoint($"http://localhost:3600").Build();
builder.Services.AddSingleton<DaprClient>(daprClient);

builder.Configuration.AddDaprSecretStore("kubernetes",
        new List<DaprSecretDescriptor> { new DaprSecretDescriptor("secret-store") }, 
        daprClient);

var connectionString = builder.Configuration.GetValue<string>("pg-connection-string");
builder.Services.AddDbContext<AuthContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddIdentity<User, Role>(o =>
        {
            o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            o.Lockout.MaxFailedAccessAttempts = 5;
            o.Lockout.AllowedForNewUsers = true;
            o.SignIn.RequireConfirmedAccount = true;
        }
    ).AddEntityFrameworkStores<AuthContext>()
    .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider); ;
builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
       o.TokenLifespan = TimeSpan.FromHours(1));

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
            await authService.SendConfirmAccountEmail(registerRequest.Email);
        }
        else if (result == ConfirmationResult.SendConfirmationLink)
        {
            await authService.SendConfirmAccountEmail(registerRequest.Email);
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


app.MapGet("/auth/validate/{userId}/{confirmation}", async (string userId, string confirmation, UserManager<User> userManager) =>
{
    var token = Base64UrlEncoder.Decode(confirmation);
    
    try
    {
        var user = await userManager.FindByIdAsync(userId);
        // if e-mail has already been confirmed, let them log in
        if (user.EmailConfirmed)
        {
            return Results.Redirect(builder.Configuration.GetValue<string>("web-login-redirect"));
        };

        var result = await userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            return Results.Redirect($"{builder.Configuration.GetValue<string>("web-login-redirect")}?confirmation=true");
        }
        else
        {
            return Results.BadRequest(builder.Configuration.GetValue<string>("web-confirmation-expired"));
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
