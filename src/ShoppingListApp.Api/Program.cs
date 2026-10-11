using ShoppingListApp.Api.Endpoints;
using ShoppingListApp.Data;

var builder = WebApplication.CreateBuilder(args);

// --

// Read connection string from configuration and ensure the database is created
var connectionString = builder.Configuration.GetConnectionString("ShoppingList")
    ?? throw new InvalidOperationException("Connection string 'ShoppingList' is missing from appsettings.json.");

builder.Services.AddShoppingListData(connectionString);

// Error handling
builder.Services.AddProblemDetails();

var app = builder.Build();

// --

app.UseExceptionHandler();
app.UseStatusCodePages();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Basic security headers on every response.
app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;
    headers["X-Content-Type-Options"] = "nosniff";
    headers["X-Frame-Options"] = "DENY";
    headers["Referrer-Policy"] = "no-referrer";
    await next();
});

// Serves the built Vue app from wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

await app.Services.EnsureShoppingListDatabaseAsync();

// Map the endpoints defined in ShoppingListEndpoints
app.MapShoppingListEndpoints();

// Unknown /api addresses get a 404 instead of the Vue page.
app.MapFallback("/api/{**path}", () => TypedResults.NotFound());
app.MapFallbackToFile("index.html");

app.Run();

 // For integration tests
public partial class Program
{
    
    
}