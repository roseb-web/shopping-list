using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShoppingListApp.Core.Services;
using ShoppingListApp.Data.Services;

namespace ShoppingListApp.Data;

public static class DataServiceExtensions
{
    public static IServiceCollection AddShoppingListData(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ShoppingListDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IShoppingListService, ShoppingListService>();

        return services;
    }
}