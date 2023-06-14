using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace GymBackend.API.Authentication
{
    public static class KeyHandler
    {
        public static SecurityKey ReadRSAKey(string path)
        {
            var pem = File.ReadAllText(path);
            var rsaPubKey = RSA.Create();
            rsaPubKey.ImportFromPem(pem);

            var key = new RsaSecurityKey(rsaPubKey);

            return key;
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
