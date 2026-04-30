using Testcontainers.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Database;
using Xunit;

public class PostgresContainerFixture : IAsyncLifetime
{
    // Zmieniamy typ na dedykowany dla Postgresa
    private PostgreSqlContainer _container = default!;

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        // Używamy dedykowanego Buildera - to usunie błędy z Twojego screena
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithDatabase("bankdb_tests")
            .WithUsername("admin")
            .WithPassword("secretpassword")
            // Porty i WaitStrategy są teraz konfigurowane automatycznie pod maską!
            .Build();

        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);
        await db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.StopAsync();
    }
}

[CollectionDefinition("postgres")]
public class PostgresCollection : ICollectionFixture<PostgresContainerFixture> { }