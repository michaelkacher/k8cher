using Microsoft.EntityFrameworkCore.Design;

namespace k8cher.auth;

public class AuthContext : IdentityDbContext<User, Role, long>
{
    public AuthContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

}

public class BloggingContextFactory : IDesignTimeDbContextFactory<AuthContext>
{
    public AuthContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json")
                       .Build();

        var connectionString = configuration.GetValue<string>("pg-connection-string");

        var optionsBuilder = new DbContextOptionsBuilder<AuthContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AuthContext(optionsBuilder.Options);
    }
}

[Keyless]
public class User : Microsoft.AspNetCore.Identity.IdentityUser<long>
{
}

public class Role : Microsoft.AspNetCore.Identity.IdentityRole<long>
{
}