using Microsoft.AspNetCore.DataProtection.KeyManagement;
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
                var pem = "-----BEGIN PRIVATE KEY-----\r\nMIIEuwIBADANBgkqhkiG9w0BAQEFAASCBKUwggShAgEAAoIBAQC8CMUqPQUTq8AL\r\nf1BtqF/gjAskpEBSzQJe3Lt5ZT+u188sdWQad2iElS8vCRq/pMcUDhPhn5wrKFFK\r\n6ZAW3a5L8+OnWLlnGXB+IGTCsoxWQxY2JH/jM7MO084lJc7M49UFLLzXmu3M6VD8\r\n/R18Xpf+ikKQ6++7Z/ZizEK7pMjLXeTR/BwdhgMUQ9ONVtgopS/K8Lm0mF/HzLO4\r\nvw6mn1hrHdAJud1MsBMuX4MsaUV75MMcFkSEaI+8K6WQe/Y9XO66s+Epi64XQ33k\r\nMpi0kmUQQI8+XAUO7k8gopw6YG8f0jXU6MHe5PaNcL001z2B0+AZcGQdyKNSlO7U\r\n3elx8dF/AgMBAAECgf93Yp89N02/U2aMOCWWHgStXx6VXXaYY4eTil8XSQObxw2z\r\nmBzS2f3E6I0cDX216F7PvjTri+t34cANeSlaKBPtoE1qh5OqqckPkzn//blkU1Kt\r\nVVZYYNHju/194Cxdy7qs5Xg0JJWFlCLIVQu6nn2a+3+4y4T0xNT3iXuNnc/fTkPj\r\nR8uzoYPSOETwqwQ/+LutJAvdkvysnx0bvYdjEmV0ZdhoXBDYKUHJX42P7U9pvd2/\r\nJYlGydoAjtR89a9T1i9WYiGXVApWOJNOtcxrx2jxCCU34axMvCtN0vednjA88lm1\r\nvJV24FUqIXbigJgh/6RxpFJsn1gkzgPWIYNLVZ0CgYEA+lF0RKzc6hEiDm2M8VnC\r\n1AYvwG1rlTBZsxcR8JRgpHWxu3B/TkFODgQbhIqCG3zgY79lI4O+Hm+PTo7wmQox\r\nIZaLd0sDsmKDzC6CAti+BjGeI2naAnKAXLvEkmdxKD1zqn6ndyHd8XlKHCtLYW6u\r\nSHyU76c4JZf1j7O2vtFGUiUCgYEAwE1luLlqOqqoKeyS8HtdbIunbE5LCwbyD4Pz\r\nHV3gClpLUgvZZErwVbK9HNEG5XvVJ8BmiFFERol8MNGN/VWRDdn2aNaoMIYAL9J4\r\n0jHsQwXhcRMea8mOYLVULUsROjIypsdcM+Y6pyT/ffQVlDMZWKwAhWZ+FOMuIZu1\r\nwIcUmdMCgYATRe5D02N3CjarEbhGZPjhRlCq6kHcTMq7RU12TpZaU02J9xF2PwT5\r\ng0tzGw8Fesn0JCpvX5dl1IUeMVdJEUXTJjo6xyXTuE4ZjIMqIPIQnAVnCKFmitZj\r\nTXnHI/vMc17Sg8n1HEBwdTxFivfU2qbEogL9qMxj6J1ZoSay7n4RmQKBgFApM0kA\r\niYaxEu5+/nWeh0AzKvRb4q5TE95h25sXQcgvAYzPAazsDCk9G9YCUESjDSTH8DXM\r\n4pAgyZM0F2NOIuxKOoxTRsMLaNKGgvTj+J+ruOFA3gKCf4ZSsCc9To4a1MBvBEFi\r\nLHr2UkZ30/L2Jg+vi3wQUGh2lwlNdkMuS+kfAoGBAKKtR6+q1Jz1LyZpdLTHq8+s\r\nGEwC8iCVKbvnVxluLCp9sLvHoNb6maQllpeIUVbO8v5PFh9CBUHbO7L8XCiGgu9w\r\nAcIyFUEmyNEu1jR+Oity6h0XQsQIzPYRn0Rls4yMRuIr4xliy3uQH2nHSXmb8ZZ4\r\nh0zYx++IoTYeFbwVZsTP\r\n-----END PRIVATE KEY-----\r\n";
                var rsaPubKey = RSA.Create();
                rsaPubKey.ImportFromPem(pem);

                var key = new RsaSecurityKey(rsaPubKey);

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
