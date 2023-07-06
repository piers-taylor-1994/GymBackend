using GymBackend.API.Authentication;
using GymBackend.Core.Contracts.Auth;
using GymBackend.Core.Contracts.Patch;
using GymBackend.Core.Domains.User;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;

namespace GymBackend.Service.Auth
{
    // All password hashing code taken from
    // https://github.com/dotnet/AspNetCore/blob/main/src/Identity/Extensions.Core/src/PasswordHasher.cs
    public class AuthManager : IAuthManager
    {
        // Change these and all passwords become invalid
        private const int iterCount = 10000;
        private const int saltSize = 256;
        private const KeyDerivationPrf prf = KeyDerivationPrf.HMACSHA512;
        private const int bytesRequested = 256 / 8;
        private readonly IConfiguration configuration;
        private readonly IAuthStorage storage;
        private readonly IPatchStorage patchStorage;

        public AuthManager(IConfiguration configuration, IAuthStorage storage, IPatchStorage patchStorage)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
            this.patchStorage = patchStorage ?? throw new ArgumentNullException(nameof(patchStorage));
        }

        private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint)(buffer[offset + 0]) << 24)
                | ((uint)(buffer[offset + 1]) << 16)
                | ((uint)(buffer[offset + 2]) << 8)
                | ((uint)(buffer[offset + 3]));
        }

        private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
        }

        private static byte[] HashPassword(string password, RandomNumberGenerator rng, KeyDerivationPrf prf, int iterCount, int saltSize, int numBytesRequested)
        {
            byte[] salt = new byte[saltSize];
            rng.GetBytes(salt);
            byte[] subKey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);

            var outputBytes = new byte[13 + salt.Length + subKey.Length];
            outputBytes[0] = 0x01; // format marker

            WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
            WriteNetworkByteOrder(outputBytes, 5, (uint)iterCount);
            WriteNetworkByteOrder(outputBytes, 9, (uint)saltSize);

            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subKey, 0, outputBytes, 13 + saltSize, subKey.Length);

            return outputBytes;
        }

        private static bool VerifyHashedPassword(byte[] hashedPassword, string password, KeyDerivationPrf prf, int iterCount)
        {
            int saltLength = (int)ReadNetworkByteOrder(hashedPassword, 9);

            if (saltLength < 128 / 8)
            {
                return false;
            }
            byte[] salt = new byte[saltLength];
            Buffer.BlockCopy(hashedPassword, 13, salt, 0, salt.Length);

            int subKeyLength = hashedPassword.Length - 13 - salt.Length;
            if (subKeyLength < 128 / 8)
            {
                return false;
            }
            byte[] expectedSubKey = new byte[subKeyLength];
            Buffer.BlockCopy(hashedPassword, 13 + salt.Length, expectedSubKey, 0, expectedSubKey.Length);

            byte[] actualSubKey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, subKeyLength);

            var match = CryptographicOperations.FixedTimeEquals(actualSubKey, expectedSubKey);

            return match;
        }

        public string GetPasswordHashAsync(string password)
        {
            var passwordBytes = HashPassword(password, RandomNumberGenerator.Create(), prf, iterCount, saltSize, bytesRequested);
            return Convert.ToBase64String(passwordBytes);
        }

        public async Task<AuthUser?> LogonAsync(string username, string password)
        {
            var authUser = await storage.FindUserAsync(username);

            if (authUser == null) return null;

            var hashBytes = Convert.FromBase64String(authUser.PasswordHash);

            var match = VerifyHashedPassword(hashBytes, password, prf, iterCount);

            if (!match) return null;

            return authUser;
        }

        public async Task<string> IssueToken(AuthUser user)
        {
            var authConfig = configuration.GetRequiredSection("Authentication");

            var keyType = authConfig.GetValue<string>("KeyType");
            SecurityKey key = keyType switch
            {
                "pem" => KeyHandler.ReadRSAKey(authConfig.GetValue<string>("KeyPath")),
                "x509" => KeyHandler.ReadX509Key(authConfig.GetValue<string>("KeyPath"), authConfig.GetValue<string>("KeyPassword")),
                _ => throw new Exception("Auth key not found")
            };

            var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
            var header = new JwtHeader(credentials);

            var audience = authConfig.GetValue<string>("Audience");
            var issuer = authConfig.GetValue<string>("Issuer");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var userName = await storage.GetNameByIdAsync(user.Id);
            var userJSON = JsonSerializer.Serialize(userName, jsonOptions);
            var userPatch = JsonSerializer.Serialize(await patchStorage.GetUserPatchReadAsync(user.Id), jsonOptions);
            claims.Add(new Claim("name", userJSON));
            claims.Add(new Claim("username", await storage.GetUsernameAsync(user.Id) ?? ""));
            claims.Add(new Claim("patch", userPatch));

            var payload = new JwtPayload(issuer, audience, claims, DateTime.UtcNow, DateTime.UtcNow.AddDays(30));

            var handler = new JwtSecurityTokenHandler();
            var token = new JwtSecurityToken(header, payload);

            return handler.WriteToken(token);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await storage.GetUsersAsync();
        }

        public async Task<AuthUser?> GetAuthUser(string username)
        {
            return await storage.FindUserAsync(username) ?? null;
        }
    }
}
