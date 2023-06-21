using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace GymBackend.API.Authentication
{
    public static class KeyHandler
    {
        public static SecurityKey ReadRSAKey(string path)
        {
            try
            {
                CspParameters cspParams = new();
                cspParams.KeyContainerName = Guid.NewGuid().ToString().ToUpperInvariant();
                cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider RSA = new(cspParams);

                var pem = File.ReadAllText(path);
                RSA.ImportFromPem(pem);

                var key = new RsaSecurityKey(RSA);

                return key;
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot ReadRSAKey", ex);
            }
        }

        public static SecurityKey ReadX509Key(string path, string password)
        {
            var cert = new X509Certificate2(path, password);
            var rsaPrivate = cert.GetRSAPrivateKey();
            var key = new RsaSecurityKey(rsaPrivate);

            return key;
        }
    }
}
