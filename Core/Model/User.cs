namespace Challenge_1_Build_APIS.Core.Model
{
    public class User

    {

        public string? Email { get; set; }
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpire { get; set; }
    }
}
