using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ShoppingListApp.Data;

namespace ShoppingListApp.Tests.Helpers;

internal class TestDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    private readonly DbContextOptions<ShoppingListDbContext> _options;

    public TestDatabase()
    {
        _connection = new SqliteConnection(GetInMemoryConnectionString());
        _connection.Open();

        _options = new DbContextOptionsBuilder<ShoppingListDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var dbContext = new ShoppingListDbContext(_options);
        dbContext.Database.EnsureCreated();
    }

    public ShoppingListDbContext CreateDbContext()
    {
        return new ShoppingListDbContext(_options);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    public static string GetInMemoryConnectionString()
    {
        return "DataSource=:memory:";
    }
}