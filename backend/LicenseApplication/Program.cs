using Microsoft.EntityFrameworkCore;
using LicenseApplication.Data;
using LicenseApplication.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/license-application-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add controllers
builder.Services.AddControllers();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5000", "http://127.0.0.1:5000", "http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Configure Entity Framework with In-Memory Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("LicenseApplicationDb"));

// Register services
builder.Services.AddScoped<ApplicationService>();

// Configure file storage
var fileStoragePath = Path.Combine(builder.Environment.ContentRootPath, "uploads");
builder.Services.AddScoped<FileStorageService>(sp =>
{
    var context = sp.GetRequiredService<ApplicationDbContext>();
    var logger = sp.GetRequiredService<ILogger<FileStorageService>>();
    return new FileStorageService(fileStoragePath, context, logger);
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSerilogRequestLogging();

app.UseCors("AllowFrontend");

// Serve static files from wwwroot
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

// Serve index.html for the root path
app.MapFallbackToFile("index.html");

Log.Information("Starting License Application API");

app.Run();
