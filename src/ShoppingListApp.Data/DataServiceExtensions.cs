using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ShoppingListApp.Data;

public static class DataServiceExtensions
{
    public static IServiceCollection AddShoppingListData(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ShoppingListDbContext>(options =>
            options.UseSqlite(connectionString));

        return services;
    }
}