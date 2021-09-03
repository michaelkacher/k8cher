var builder = WebApplication.CreateBuilder(args);


var daprHttpPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3600";

builder.Services.AddDaprClient(builder => builder.UseHttpEndpoint($"http://localhost:{daprHttpPort}"));

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


// todo - mbk: if having trouble getting secrets can try writing somethting that tries from dapr secret store and falls back on config
//var secretValues = await client.GetSecretAsync(
//                    "kubernetes", // Name of the Dapr Secret Store
//                    "super-secret", // Name of the k8s secret
//                    new Dictionary<string, string>() { { "namespace", "default" } }); // Namespace where the k8s secret is deployed

//// Get the secret value
//var secretValue = secretValues["super-secret"];




var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapActorsHandlers();


app.MapGet("store/hello", async (ClaimsPrincipal user) => {
    var userId = user.FindFirst("id");
    Console.WriteLine($"User has id of {userId}");
    return Results.Ok("Hello World");
}).RequireAuthorization();


app.MapPost("/store/set", async (SetStoreRequest setStoreRequest, ClaimsPrincipal user) =>
{
    var userId = user.FindFirst("id");
    var actorId = new ActorId(setStoreRequest.Name + "-" + userId);
    var proxy = ActorProxy.Create<ISvelteStoreActor>(actorId, nameof(SvelteStoreActor));
    try
    {
        using var stream = new MemoryStream();
        Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
        setStoreRequest.Json.WriteTo(writer);
        writer.Flush();
        var json = Encoding.UTF8.GetString(stream.ToArray());
        Console.WriteLine($"JSON value: {json}");


        var success = await proxy.SetState(json);

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error post setstore: {ex.Message}");
    }

    return Results.Ok();


});


app.MapPost("/store/update", async (UpdateStorePropertyRequest updateStorePropertyRequest, ClaimsPrincipal user) =>
{
    var userId = user.FindFirst("id");
    var actorId = new ActorId(updateStorePropertyRequest.Name + "-" + userId);
    var proxy = ActorProxy.Create<ISvelteStoreActor>(actorId, nameof(SvelteStoreActor));
    var success = await proxy.UpdateProperty(updateStorePropertyRequest.PropertyName, updateStorePropertyRequest.Json);

    return Results.Ok(success);
});

app.MapGet("store/get", async (string storeName, ClaimsPrincipal user) =>
{
    var userId = user.FindFirst("id");
    var actorId = new ActorId(storeName + "-" + userId);
    var proxy = ActorProxy.Create<ISvelteStoreActor>(actorId, nameof(SvelteStoreActor));
    var success = await proxy.GetState();

    return Results.Ok(success);
});

app.Run();
