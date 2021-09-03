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

app.Run();
