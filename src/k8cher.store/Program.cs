using Dapr.Client;
using Dapr.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

var daprHttpPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3600";

var daprClient = new DaprClientBuilder().UseHttpEndpoint($"http://localhost:{daprHttpPort}").Build();
builder.Services.AddSingleton<DaprClient>(daprClient);
        
//builder.Services.AddDaprClient(builder => builder.UseHttpEndpoint($"http://localhost:{daprHttpPort}"));

var secretDescriptors = new List<DaprSecretDescriptor>
                            {
                                new DaprSecretDescriptor("secret-store")
                            };
builder.Configuration.AddDaprSecretStore("kubernetes", secretDescriptors, daprClient);


builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<SvelteStoreActor>();
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    // The secrets are registered into configuration by Dapr using the kubernetes store
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

app.MapGet("/store/{storeName}/get", async (string storeName, ClaimsPrincipal user) =>
{
    var userId = user.FindFirst("id");
    var actorId = new ActorId(storeName + "-" + userId);
    var proxy = ActorProxy.Create<ISvelteStoreActor>(actorId, nameof(SvelteStoreActor));
    try
    {
        var result = await proxy.GetState();
        var json = JsonDocument.Parse(result);

        return Results.Ok(json);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error post setstore: {ex.Message}");
    }

    return Results.BadRequest();
}).RequireAuthorization();

app.MapPost("/store/{storeName}/set", async (string storeName, JsonDocument jsonDocument, ClaimsPrincipal user) =>
{
    var userId = user.FindFirst("id");
    var actorId = new ActorId(storeName + "-" + userId);
    var proxy = ActorProxy.Create<ISvelteStoreActor>(actorId, nameof(SvelteStoreActor));
    try
    {
        using var stream = new MemoryStream();
        Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { });
        jsonDocument.WriteTo(writer);
        writer.Flush();
        var json = Encoding.UTF8.GetString(stream.ToArray());

        var success = await proxy.SetState(json);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error post setstore: {ex.Message}");
    }

    return Results.Ok();
}).RequireAuthorization();

app.Run();
