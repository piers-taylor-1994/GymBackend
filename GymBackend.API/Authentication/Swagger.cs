using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GymBackend.API.Authentication
{
    public class AuthResponseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var anonAttributes = context?.MethodInfo?.DeclaringType?.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AllowAnonymousAttribute>();

            if (anonAttributes?.Any() == false) 
            {
                var scheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    },
                    Scheme = "bearer",
                    Name = "JWT Authentication",
                    BearerFormat = "JWT",
                    Description = "JWT Bearer token",
                    Type = SecuritySchemeType.Http,
                    In = ParameterLocation.Header
                };

                operation.Security = new List<OpenApiSecurityRequirement> { new OpenApiSecurityRequirement { { scheme, Array.Empty<string>() } } };
            }
        }
    }

    public static class Swagger
    {
        public static void Configure(SwaggerGenOptions options) 
        {
            var scheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "bearer",
                Name = "JWT Authentication",
                BearerFormat = "JWT",
                Description = "JWT Bearer token",
                Type = SecuritySchemeType.Http,
                In = ParameterLocation.Header
            };

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, scheme);
            options.OperationFilter<AuthResponseOperationFilter>();
        }
    }
}
