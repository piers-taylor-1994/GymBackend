using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GymBackend.API.Authentication
{
    public static class Configuration
    {
        public static void EnableAuth(IServiceCollection services, ConfigurationManager configManager)
        {
            var authConfig = configManager.GetRequiredSection("Authentication");
            var audience = authConfig.GetValue<string>("Audience");
            var issuer = authConfig.GetValue<string>("Issuer");

            //https://www.claudiobernasconi.ch/2016/04/17/creating-a-self-signed-x509-certificate-using-openssl-on-windows/
            var keyType = authConfig.GetValue<string>("KeyType");
            SecurityKey key = keyType switch
            {
                "pem" => KeyHandler.ReadRSAKey(authConfig.GetValue<string>("KeyPath")),
                "x509" => KeyHandler.ReadX509Key(authConfig.GetValue<string>("KeyPath"), authConfig.GetValue<string>("KeyPassword")),
                _ => throw new Exception("Auth key not found")
            };

            var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            var parameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidAudience = audience,
                ValidIssuer = issuer
            };

            services.AddAuthorization();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Audience = audience;
                options.ClaimsIssuer = issuer;
                options.TokenValidationParameters = parameters;
                options.MapInboundClaims = false;
            });
        }
    }
}
