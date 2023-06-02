using GymBackend.Core.Contracts.Auth;
using GymBackend.Service.Auth;
using GymBackend.Storage.Auth;
using YOTApp.Storage;

var origins = "AllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
var configManager = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDatabase, Database>((services) =>
{
    var config = services.GetService<IConfiguration>();
    return new Database(config.GetConnectionString("Database"));
});

builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IAuthStorage, AuthStorage>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
