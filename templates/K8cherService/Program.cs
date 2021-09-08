var builder = WebApplication.CreateBuilder(args);

var daprHttpPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3600";

builder.Services.AddDaprClient(builder => builder.UseHttpEndpoint($"http://localhost:{daprHttpPort}"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("signing-key"))),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration.GetValue<string>("jwt-issuer"),
        ValidAudience = builder.Configuration.GetValue<string>("jwt-audience"),
        ValidateLifetime = true
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapActorsHandlers();

app.MapGet("K8cherService/hello", async (ClaimsPrincipal user) => {
    return Results.Ok("Insecure Hello");
}).RequireAuthorization();

app.MapGet("K8cherService/securehello", async (ClaimsPrincipal user) => {
    var userId = user.FindFirst("id");
    Console.WriteLine($"User has id of {userId}");
    return Results.Ok("Secure Hello World");
}).RequireAuthorization();


app.Run();
