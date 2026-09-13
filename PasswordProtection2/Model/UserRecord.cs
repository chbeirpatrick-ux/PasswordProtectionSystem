namespace PasswordProtectionSystem.Model
{
    public class PlainPasswordRecord
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string PasswordPlain { get; set; }
    }

    public class HashedPasswordRecord
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }


    public class SaltedPasswordRecord
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Salt { get; set; }
        public string PasswordSaltedHash { get; set; }
        public DateTime Timestamp { get; set; }
    }
}