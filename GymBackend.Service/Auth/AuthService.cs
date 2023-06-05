using GymBackend.Core.Contracts.Auth;
using GymBackend.Core.Domains;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace GymBackend.Service.Auth
{
    // All password hashing code taken from
    // https://github.com/dotnet/AspNetCore/blob/main/src/Identity/Extensions.Core/src/PasswordHasher.cs
    public class AuthService : IAuthService
    {
        // Change these and all passwords become invalid
        private const int iterCount = 10000;
        private const int saltSize = 256;
        private const KeyDerivationPrf prf = KeyDerivationPrf.HMACSHA512;
        private const int bytesRequested = 256 / 8;

        private readonly IAuthStorage storage;

        public AuthService(IAuthStorage storage)
        {
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
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

        public async Task<List<User>> GetUsersAsync()
        {
            return await storage.GetUsersAsync();
        }

        
    }
}
