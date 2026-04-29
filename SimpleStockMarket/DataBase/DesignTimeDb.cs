using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Database;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MainDbContext>
{
	public MainDbContext CreateDbContext(string[] args)
    {
        // read from appsettings
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables();

        var config = builder.Build();

        var connectionString = config.GetConnectionString("DefaultConnection")
                                ?? "Host=localhost;Database=bankdb;Username=admin;Password=secretpassword";


        var options = new DbContextOptionsBuilder<MainDbContext>();
        options.UseNpgsql(connectionString);

        return new MainDbContext(options.Options);
    }
}
