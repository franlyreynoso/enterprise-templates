using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EnterpriseTemplate.Infrastructure.Persistence;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var cfg = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile("appsettings.Development.json", true)
            .AddEnvironmentVariables()
            .Build();

        var cs = cfg.GetConnectionString("Default")
                 ?? "Host=localhost;Port=5432;Database=enterprisetemplate_dev;Username=app;Password=app";

        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseNpgsql(cs);

        return new AppDbContext(builder.Options);
    }
}
