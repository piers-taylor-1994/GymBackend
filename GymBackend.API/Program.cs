using GymBackend.Core.Contracts.Auth;
using GymBackend.Service.Auth;
using GymBackend.Storage.Auth;
using YOTApp.Storage;

var origins = "AllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
var configManager = builder.Configuration;



//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(origins, builder =>
//    {
//        var corsConfig = configManager.GetSection("Cors:Origins");
//        Console.WriteLine(corsConfig.ToString());
//        var withOrigins = corsConfig.Get<string[]>();
//        builder
//            .WithOrigins(withOrigins)
//            .AllowCredentials()
//            .AllowAnyHeader()
//            .AllowAnyMethod();
//    });
//});

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

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IAuthStorage, AuthStorage>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors((policy) =>
{
    policy.AllowAnyOrigin();
    policy.AllowAnyHeader();
    policy.AllowAnyMethod();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
