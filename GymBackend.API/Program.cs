using GymBackend.API.Authentication;
using GymBackend.Core.Contracts;
using GymBackend.Core.Contracts.Auth;
using GymBackend.Core.Contracts.Booking;
using GymBackend.Core.Contracts.Patch;
using GymBackend.Core.Contracts.Swimming;
using GymBackend.Core.Contracts.Workouts;
using GymBackend.Service.Auth;
using GymBackend.Service.Booking;
using GymBackend.Service.Patch;
using GymBackend.Service.Workouts;
using GymBackend.Storage;
using GymBackend.Storage.Auth;
using GymBackend.Storage.Booking;
using GymBackend.Storage.Patch;
using GymBackend.Storage.Workouts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var origins = "AllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", true, true);
var configManager = builder.Configuration;

Configuration.EnableAuth(builder.Services, configManager);

builder.Services.AddCors(options =>
{
    options.AddPolicy(origins, builder =>
    {
        var corsConfig = configManager.GetSection("Cors:Origins");
        var withOrigins = corsConfig.Get<string[]>();
        builder
            .WithOrigins(withOrigins)
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add services to the container.
builder.Services
    .AddControllers(c =>
    {
        // This makes endpoints requirer a bearer token
        if (!builder.Environment.IsDevelopment())
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            c.Filters.Add(new AuthorizeFilter(policy));
        }
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(Swagger.Configure);

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddScoped<IDatabase, Database>((services) =>
{
    var config = services.GetService<IConfiguration>();
    return new Database(config.GetConnectionString("Database"));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<IAuthManager, AuthManager>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IAuthStorage, AuthStorage>();
builder.Services.AddTransient<IWorkoutsService, WorkoutsService>();
builder.Services.AddTransient<IWorkoutsStorage, WorkoutsStorage>();
builder.Services.AddTransient<IPatchService, PatchService>();
builder.Services.AddTransient<IPatchStorage, PatchStorage>();
builder.Services.AddTransient<IBookingService, BookingService>();
builder.Services.AddTransient<IBookingStorage, BookingStorage>();
builder.Services.AddTransient<ISwimmingService, SwimmingService>();
builder.Services.AddTransient<ISwimmingStorage, SwimmingStorage>();

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(origins);

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
