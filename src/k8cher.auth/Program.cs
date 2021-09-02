var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetValue<string>("pg-connection-string");

builder.Services.AddDbContext<AuthContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddIdentity<User, Role>().AddEntityFrameworkStores<AuthContext>();

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

app.MapPost("/auth/register", async (RegisterRequest registerRequest, UserManager<User> userManager) =>
{
    if (registerRequest.User == null) throw new ArgumentNullException(nameof(registerRequest.User));

    var result = await userManager.CreateAsync(registerRequest.User, registerRequest.Password);

    if (result.Succeeded)
    {
        Console.WriteLine($"New user created: {registerRequest.User}");
        return Results.Ok();
    }

    Console.WriteLine($"Failed to create user: {registerRequest.User}");

    return Results.BadRequest("Invalid request");
});


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
