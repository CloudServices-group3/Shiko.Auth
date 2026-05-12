using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Data.Identity;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCorsConfiguration(); // File for configuring CORS policies and allowed origins.

builder.Services.AddIdentityConfiguration(builder.Configuration); // File in the Infrastructure layer with configuration for JWT and more.

builder.Services.AddJwtConfiguration(builder.Configuration);

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

builder.Services.AddInfrastructure
    (
        builder.Configuration,
        builder.Environment    
    );

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("Frontend"); // Enable CORS using the "Frontend" policy.

using ( var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var dbContext = services.GetRequiredService<DataContext>();

    // The database is only migrated if it is a relational database.
    if (dbContext.Database.IsRelational())
    {
        await dbContext.Database.MigrateAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Authentication
app.UseAuthorization();

app.MapControllers();

app.Run();
