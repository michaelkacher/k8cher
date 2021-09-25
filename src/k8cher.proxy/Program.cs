using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddCors();

var app = builder.Build();
app.MapReverseProxy();
app.UseCors(p =>
{
    p.AllowAnyOrigin();
    p.AllowAnyHeader();
});

app.MapGet("healthz", async () => {
    return Results.Ok("Healthy");
});

app.Run();
