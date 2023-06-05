namespace GymBackend.Core.Domains.User
{
    public class AuthUser
    {
        public Guid Id { get; set; }
        public string PasswordHash { get; set; }
    }
}
