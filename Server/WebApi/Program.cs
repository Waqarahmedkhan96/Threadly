using EfcRepositories;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RepositoryContracts;
using System.IO;
using System.Text.Json;                     // JSON settings
using System.Text.Json.Serialization;      // ReferenceHandler

// explicit usings
using EfAppContext = EfcRepositories.AppContext; // alias for DB context

// WebApi.GlobalExceptionHandler
using WebApi.GlobalExceptionHandler;

var builder = WebApplication.CreateBuilder(args);

// Controllers + JSON settings
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;              // ignore case
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // camelCase
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;   // avoid cycles
    });

// Register EF DbContext (SQLite app.db)
builder.Services.AddScoped<EfAppContext>(); // use alias, avoid System.AppContext

// Swagger (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI (Now Efc Repositories)
builder.Services.AddScoped<IUserRepository, EfcUserRepository>();        // users from DB
builder.Services.AddScoped<IPostRepository, EfcPostRepository>();        // posts from DB
builder.Services.AddScoped<ICommentRepository, EfcCommentRepository>();  // comments from DB

// Register Global Exception Middleware
builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();

var app = builder.Build();

// seed default user if missing and apply any pending migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EfAppContext>();
    db.Database.Migrate();

    if (!db.Users.Any(u => u.Username == "Lala"))
    {
        db.Users.Add(new User("Lala", "3333"));
        db.SaveChanges();
    }
}

//plug middleware in early so it catches downstream exceptions
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Swagger
}

var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
if (!Directory.Exists(uploadsRoot))
{
    Directory.CreateDirectory(uploadsRoot);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsRoot),
    RequestPath = "/uploads"
});

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
