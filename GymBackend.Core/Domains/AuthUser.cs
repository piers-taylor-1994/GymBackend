namespace GymBackend.Core.Domains
{
    public class AuthUser
    {
        public Guid Id { get; set; }
        public string PasswordHash { get; set; }
    }
}
